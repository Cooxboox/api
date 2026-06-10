using Cooxboox.Core.IngredientCategories.Events;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories;

public class IngredientCategory : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "IngredientCategory";

  public new IngredientCategoryId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, IngredientCategoryLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public IngredientCategory() : base()
  {
  }

  public IngredientCategory(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(IngredientCategoryId.NewId(kitchen.Id), name, actorId)
  {
  }

  public IngredientCategory(IngredientCategoryId ingredientCategoryId, Name name, ActorId? actorId = null)
    : base(ingredientCategoryId.StreamId)
  {
    Raise(new IngredientCategoryCreated(name), actorId);
  }
  protected virtual void Handle(IngredientCategoryCreated @event)
  {
    _name = @event.Name;
  }

  public void Annotate(Notes? notes, ActorId? actorId = null)
  {
    if (!Equals(Notes, notes))
    {
      Raise(new IngredientCategoryAnnotated(notes), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryAnnotated @event)
  {
    Notes = @event.Notes;

    // TODO(fpion): invariant status
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new IngredientCategoryDeleted(), actorId);
    }
  }

  public IngredientCategoryLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

  public bool HasLocale(Language language) => _locales.ContainsKey(language);

  public void Publish(ActorId? actorId = null)
  {
    PublishInvariant(actorId);

    foreach (Language language in _locales.Keys)
    {
      PublishLocale(language, actorId);
    }
  }
  public void PublishInvariant(ActorId? actorId = null)
  {
    if (_status != ContentStatus.Latest)
    {
      Raise(new IngredientCategoryPublished(Language: null), actorId);
    }
  }
  public void PublishLocale(Language language, ActorId? actorId = null)
  {
    if (_status == ContentStatus.Unpublished)
    {
      throw new InvariantNotPublishedException(this);
    }
    else if (!_statuses.TryGetValue(language, out ContentStatus status))
    {
      throw new LocaleNotFoundException(this, language);
    }
    else if (status != ContentStatus.Latest)
    {
      Raise(new IngredientCategoryPublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryPublished @event)
  {
    if (@event.Language is null)
    {
      _status = ContentStatus.Latest;
    }
    else
    {
      _statuses[@event.Language] = ContentStatus.Latest;
    }
  }

  public void RemoveLocale(Language language, ActorId? actorId = null)
  {
    if (HasLocale(language))
    {
      Raise(new IngredientCategoryLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Name.Equals(name))
    {
      Raise(new IngredientCategoryRenamed(name), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryRenamed @event)
  {
    _name = @event.Name;

    // TODO(fpion): invariant status
  }

  public void SetLocale(Language language, IngredientCategoryLocale locale, ActorId? actorId = null)
  {
    IngredientCategoryLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new IngredientCategoryLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryLocaleChanged @event)
  {
    _locales[@event.Language] = @event.Locale;

    if (!_statuses.TryGetValue(@event.Language, out ContentStatus status))
    {
      _statuses[@event.Language] = ContentStatus.Unpublished;
    }
    else if (status == ContentStatus.Latest)
    {
      _statuses[@event.Language] = ContentStatus.Published;
    }
  }

  public IngredientCategoryLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out IngredientCategoryLocale? locale) ? locale : null;

  public void Unpublish(ActorId? actorId = null)
  {
    UnpublishInvariant(actorId);

    foreach (Language language in _locales.Keys)
    {
      UnpublishLocale(language, actorId);
    }
  }
  public void UnpublishInvariant(ActorId? actorId = null)
  {
    if (_status != ContentStatus.Unpublished)
    {
      Raise(new IngredientCategoryUnpublished(Language: null), actorId);
    }
  }
  public void UnpublishLocale(Language language, ActorId? actorId = null)
  {
    if (!_statuses.TryGetValue(language, out ContentStatus status))
    {
      throw new LocaleNotFoundException(this, language);
    }
    else if (status != ContentStatus.Unpublished)
    {
      Raise(new IngredientCategoryUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientCategoryUnpublished @event)
  {
    if (@event.Language is null)
    {
      _status = ContentStatus.Unpublished;
    }
    else
    {
      _statuses[@event.Language] = ContentStatus.Unpublished;
    }
  }


  public override string ToString() => $"{Name} | {base.ToString()}";
}

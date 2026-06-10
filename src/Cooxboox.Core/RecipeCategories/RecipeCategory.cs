using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.RecipeCategories.Events;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories;

public class RecipeCategory : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "RecipeCategory";

  public new RecipeCategoryId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, RecipeCategoryLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public RecipeCategory() : base()
  {
  }

  public RecipeCategory(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(RecipeCategoryId.NewId(kitchen.Id), name, actorId)
  {
  }

  public RecipeCategory(RecipeCategoryId recipeCategoryId, Name name, ActorId? actorId = null)
    : base(recipeCategoryId.StreamId)
  {
    Raise(new RecipeCategoryCreated(name), actorId);
  }
  protected virtual void Handle(RecipeCategoryCreated @event)
  {
    _name = @event.Name;
  }

  public void Annotate(Notes? notes, ActorId? actorId = null)
  {
    if (!Equals(Notes, notes))
    {
      Raise(new RecipeCategoryAnnotated(notes), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryAnnotated @event)
  {
    Notes = @event.Notes;

    // TODO(fpion): invariant status
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RecipeCategoryDeleted(), actorId);
    }
  }

  public RecipeCategoryLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new RecipeCategoryPublished(Language: null), actorId);
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
      Raise(new RecipeCategoryPublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryPublished @event)
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
      Raise(new RecipeCategoryLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Name.Equals(name))
    {
      Raise(new RecipeCategoryRenamed(name), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryRenamed @event)
  {
    _name = @event.Name;

    // TODO(fpion): invariant status
  }

  public void SetLocale(Language language, RecipeCategoryLocale locale, ActorId? actorId = null)
  {
    RecipeCategoryLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new RecipeCategoryLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryLocaleChanged @event)
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

  public RecipeCategoryLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out RecipeCategoryLocale? locale) ? locale : null;

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
      Raise(new RecipeCategoryUnpublished(Language: null), actorId);
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
      Raise(new RecipeCategoryUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipeCategoryUnpublished @event)
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

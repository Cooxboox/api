using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Recipes.Events;
using Cooxboox.Core.RecipeTypes;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes;

public class Recipe : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Recipe";

  public new RecipeId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  public RecipeTypeId? RecipeTypeId { get; private set; }

  private readonly Dictionary<Language, RecipeLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public Recipe() : base()
  {
  }

  public Recipe(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(RecipeId.NewId(kitchen.Id), name, actorId)
  {
  }

  public Recipe(RecipeId recipeId, Name name, ActorId? actorId = null)
    : base(recipeId.StreamId)
  {
    Raise(new RecipeCreated(name), actorId);
  }
  protected virtual void Handle(RecipeCreated @event)
  {
    _name = @event.Name;
  }

  public void Annotate(Notes? notes, ActorId? actorId = null)
  {
    if (!Equals(Notes, notes))
    {
      Raise(new RecipeAnnotated(notes), actorId);
    }
  }
  protected virtual void Handle(RecipeAnnotated @event)
  {
    Notes = @event.Notes;

    UpdateInvariant();
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RecipeDeleted(), actorId);
    }
  }

  public RecipeLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new RecipePublished(Language: null), actorId);
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
      Raise(new RecipePublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipePublished @event)
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
      Raise(new RecipeLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(RecipeLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Name.Equals(name))
    {
      Raise(new RecipeRenamed(name), actorId);
    }
  }
  protected virtual void Handle(RecipeRenamed @event)
  {
    _name = @event.Name;

    UpdateInvariant();
  }

  public void SetLocale(Language language, RecipeLocale locale, ActorId? actorId = null)
  {
    RecipeLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new RecipeLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(RecipeLocaleChanged @event)
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

  public void SetType(RecipeType? recipeType, ActorId? actorId = null) => SetType(recipeType?.Id, actorId);
  public void SetType(RecipeTypeId? recipeTypeId, ActorId? actorId = null)
  {
    if (recipeTypeId.HasValue)
    {
      KitchenMismatchException.ThrowIfMismatch(Entity, new Entity(EntityKind, recipeTypeId.Value.EntityId, recipeTypeId.Value.KitchenId));
    }

    if (RecipeTypeId != recipeTypeId)
    {
      Raise(new RecipeTyped(recipeTypeId), actorId);
    }
  }
  protected virtual void Handle(RecipeTyped @event)
  {
    RecipeTypeId = @event.RecipeTypeId;
  }

  public RecipeLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out RecipeLocale? locale) ? locale : null;

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
      Raise(new RecipeUnpublished(Language: null), actorId);
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
      Raise(new RecipeUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipeUnpublished @event)
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

  private void UpdateInvariant()
  {
    if (_status == ContentStatus.Latest)
    {
      _status = ContentStatus.Published;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

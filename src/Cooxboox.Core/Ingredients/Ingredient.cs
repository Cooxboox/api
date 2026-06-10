using Cooxboox.Core.Ingredients.Events;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients;

public class Ingredient : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Ingredient";

  public new IngredientId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, IngredientLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public Ingredient() : base()
  {
  }

  public Ingredient(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(IngredientId.NewId(kitchen.Id), name, actorId)
  {
  }

  public Ingredient(IngredientId ingredientId, Name name, ActorId? actorId = null)
    : base(ingredientId.StreamId)
  {
    Raise(new IngredientCreated(name), actorId);
  }
  protected virtual void Handle(IngredientCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new IngredientDeleted(), actorId);
    }
  }

  public IngredientLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new IngredientPublished(Language: null), actorId);
    }
  }
  public void PublishLocale(Language language, ActorId? actorId = null)
  {
    // TODO(fpion): can we publish a locale if the invariant is not published?

    if (!_statuses.TryGetValue(language, out ContentStatus status))
    {
      throw new LocaleNotFoundException(this, language);
    }
    else if (status != ContentStatus.Latest)
    {
      Raise(new IngredientPublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientPublished @event)
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
      Raise(new IngredientUnpublished(Language: null), actorId);
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
      Raise(new IngredientUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientUnpublished @event)
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

  public void RemoveLocale(Language language, ActorId? actorId = null)
  {
    if (HasLocale(language))
    {
      Raise(new IngredientLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(IngredientLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void SetLocale(Language language, IngredientLocale locale, ActorId? actorId = null)
  {
    IngredientLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new IngredientLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(IngredientLocaleChanged @event)
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

  public IngredientLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out IngredientLocale? locale) ? locale : null;

  public void Update(Name name, Notes? notes, ActorId? actorId = null)
  {
    IngredientUpdated @event = new(
      Name.Equals(name) ? null : name,
      Equals(Notes, notes) ? null : new Optional<Notes>(notes));

    if (@event.Name is not null || @event.Notes is not null)
    {
      Raise(@event, actorId);
    }
  }
  protected virtual void Handle(IngredientUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value;
    }

    if (_status == ContentStatus.Latest)
    {
      _status = ContentStatus.Published;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

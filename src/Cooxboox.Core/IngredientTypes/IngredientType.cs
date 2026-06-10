using Cooxboox.Core.IngredientTypes.Events;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes;

public class IngredientType : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "IngredientType";

  public new IngredientTypeId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, IngredientTypeLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public IngredientType() : base()
  {
  }

  public IngredientType(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(IngredientTypeId.NewId(kitchen.Id), name, actorId)
  {
  }

  public IngredientType(IngredientTypeId ingredientTypeId, Name name, ActorId? actorId = null)
    : base(ingredientTypeId.StreamId)
  {
    Raise(new IngredientTypeCreated(name), actorId);
  }
  protected virtual void Handle(IngredientTypeCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new IngredientTypeDeleted(), actorId);
    }
  }

  public IngredientTypeLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new IngredientTypePublished(Language: null), actorId);
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
      Raise(new IngredientTypePublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientTypePublished @event)
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
      Raise(new IngredientTypeUnpublished(Language: null), actorId);
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
      Raise(new IngredientTypeUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(IngredientTypeUnpublished @event)
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
      Raise(new IngredientTypeLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(IngredientTypeLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void SetLocale(Language language, IngredientTypeLocale locale, ActorId? actorId = null)
  {
    IngredientTypeLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new IngredientTypeLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(IngredientTypeLocaleChanged @event)
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

  public IngredientTypeLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out IngredientTypeLocale? locale) ? locale : null;

  public void Update(Name name, Notes? notes, ActorId? actorId = null)
  {
    IngredientTypeUpdated @event = new(
      Name.Equals(name) ? null : name,
      Equals(Notes, notes) ? null : new Optional<Notes>(notes));

    if (@event.Name is not null || @event.Notes is not null)
    {
      Raise(@event, actorId);
    }
  }
  protected virtual void Handle(IngredientTypeUpdated @event)
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

using Cooxboox.Core.RecipeTypes.Events;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes;

public class RecipeType : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "RecipeType";

  public new RecipeTypeId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, RecipeTypeLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public RecipeType() : base()
  {
  }

  public RecipeType(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(RecipeTypeId.NewId(kitchen.Id), name, actorId)
  {
  }

  public RecipeType(RecipeTypeId ingredientTypeId, Name name, ActorId? actorId = null)
    : base(ingredientTypeId.StreamId)
  {
    Raise(new RecipeTypeCreated(name), actorId);
  }
  protected virtual void Handle(RecipeTypeCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new RecipeTypeDeleted(), actorId);
    }
  }

  public RecipeTypeLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new RecipeTypePublished(Language: null), actorId);
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
      Raise(new RecipeTypePublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipeTypePublished @event)
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
      Raise(new RecipeTypeUnpublished(Language: null), actorId);
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
      Raise(new RecipeTypeUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(RecipeTypeUnpublished @event)
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
      Raise(new RecipeTypeLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(RecipeTypeLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void SetLocale(Language language, RecipeTypeLocale locale, ActorId? actorId = null)
  {
    RecipeTypeLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new RecipeTypeLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(RecipeTypeLocaleChanged @event)
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

  public RecipeTypeLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out RecipeTypeLocale? locale) ? locale : null;

  public void Update(Name name, Notes? notes, ActorId? actorId = null)
  {
    RecipeTypeUpdated @event = new(
      Name.Equals(name) ? null : name,
      Equals(Notes, notes) ? null : new Optional<Notes>(notes));

    if (@event.Name is not null || @event.Notes is not null)
    {
      Raise(@event, actorId);
    }
  }
  protected virtual void Handle(RecipeTypeUpdated @event)
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

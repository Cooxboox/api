using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "Kitchen";

  public new KitchenId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId);

  public UserId OwnerId { get; private set; }
  public Confidentiality Confidentiality { get; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Slug? Slug { get; private set; }
  public Notes? Notes { get; private set; }

  private readonly Dictionary<Language, KitchenLocale> _locales = [];

  private ContentStatus _status = ContentStatus.Unpublished;
  private readonly Dictionary<Language, ContentStatus> _statuses = [];

  public Kitchen() : base()
  {
  }

  public Kitchen(UserId ownerId, Name name, KitchenId? kitchenId = null)
    : base((kitchenId ?? KitchenId.NewId()).StreamId)
  {
    Raise(new KitchenCreated(ownerId, name), ownerId.ActorId);
  }
  protected virtual void Handle(KitchenCreated @event)
  {
    OwnerId = @event.OwnerId;

    _name = @event.Name;
  }

  public void Annotate(Notes? notes, ActorId? actorId = null)
  {
    if (!Equals(Notes, notes))
    {
      Raise(new KitchenAnnotated(notes), actorId);
    }
  }
  protected virtual void Handle(KitchenAnnotated @event)
  {
    Notes = @event.Notes;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new KitchenDeleted(), actorId);
    }
  }

  public KitchenLocale FindLocale(Language language) => TryGetLocale(language) ?? throw new LocaleNotFoundException(this, language);

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
      Raise(new KitchenPublished(Language: null), actorId);
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
      Raise(new KitchenPublished(language), actorId);
    }
  }
  protected virtual void Handle(KitchenPublished @event)
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

  public void SetSlug(Slug? slug, ActorId? actorId = null)
  {
    if (!Equals(Slug, slug))
    {
      Raise(new KitchenSlugChanged(slug), actorId);
    }
  }
  protected virtual void Handle(KitchenSlugChanged @event)
  {
    Slug = @event.Slug;
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
      Raise(new KitchenUnpublished(Language: null), actorId);
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
      Raise(new KitchenUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(KitchenUnpublished @event)
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
      Raise(new KitchenLocaleRemoved(language), actorId);
    }
  }
  protected virtual void Handle(KitchenLocaleRemoved @event)
  {
    _locales.Remove(@event.Language);
    _statuses.Remove(@event.Language);
  }

  public void Rename(Name name, ActorId? actorId = null)
  {
    if (!Name.Equals(name))
    {
      Raise(new KitchenRenamed(name), actorId);
    }
  }
  protected virtual void Handle(KitchenRenamed @event)
  {
    _name = @event.Name;
  }

  public void SetLocale(Language language, KitchenLocale locale, ActorId? actorId = null)
  {
    KitchenLocale? existingLocale = TryGetLocale(language);
    if (existingLocale is null || !existingLocale.Equals(locale))
    {
      Raise(new KitchenLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(KitchenLocaleChanged @event)
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

  public KitchenLocale? TryGetLocale(Language language) => _locales.TryGetValue(language, out KitchenLocale? locale) ? locale : null;

  public override string ToString() => $"{Name} | {base.ToString()}";
}

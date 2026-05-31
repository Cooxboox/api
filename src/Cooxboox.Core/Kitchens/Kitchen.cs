using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Validation;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : AggregateRoot
{
  public const string EntityKind = "Kitchen";

  public new KitchenId Id => new(base.Id);
  public Guid EntityId => Id.EntityId;

  public UserId OwnerId { get; private set; }

  public Confidentiality Confidentiality { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");

  public Slug? Slug { get; private set; }

  private readonly Dictionary<Language, KitchenLocale> _locales = [];
  public IReadOnlyDictionary<Language, KitchenLocale> Locales => _locales.AsReadOnly();
  private readonly Dictionary<Language, ContentStatus> _statuses = [];
  public IReadOnlyDictionary<Language, ContentStatus> Statuses => _statuses.AsReadOnly();

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

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new KitchenDeleted(), actorId);
    }
  }

  public void Publish(ActorId? actorId = null)
  {
    foreach (Language language in _statuses.Keys)
    {
      Publish(language, actorId);
    }
  }
  public void Publish(Language language, ActorId? actorId = null)
  {
    if (!_statuses.TryGetValue(language, out ContentStatus existingStatus))
    {
      throw new KitchenLocaleNotFoundException(this, language);
    }
    else if (existingStatus != ContentStatus.Latest)
    {
      Raise(new KitchenLocalePublished(language), actorId);
    }
  }
  protected virtual void Handle(KitchenLocalePublished @event)
  {
    _statuses[@event.Language] = ContentStatus.Latest;
  }

  public void RemoveLocale(Language language, ActorId? actorId = null)
  {
    if (_locales.ContainsKey(language))
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

  public void SetConfidentiality(Confidentiality confidentiality, ActorId? actorId = null)
  {
    if (Confidentiality != confidentiality)
    {
      Raise(new KitchenConfidentialityChanged(confidentiality), actorId);
    }
  }
  protected virtual void Handle(KitchenConfidentialityChanged @event)
  {
    Confidentiality = @event.Confidentiality;
  }

  public void SetLocale(Language language, KitchenLocale locale, ActorId? actorId = null)
  {
    if (!_locales.TryGetValue(language, out KitchenLocale? existingLocale) || !existingLocale.Equals(locale))
    {
      Raise(new KitchenLocaleChanged(language, locale), actorId);
    }
  }
  protected virtual void Handle(KitchenLocaleChanged @event)
  {
    _locales[@event.Language] = @event.Locale;

    if (!_statuses.TryGetValue(@event.Language, out ContentStatus existingStatus))
    {
      _statuses[@event.Language] = ContentStatus.Unpublished;
    }
    else if (existingStatus == ContentStatus.Latest)
    {
      _statuses[@event.Language] = ContentStatus.Published;
    }
  }

  public void SetSlug(Slug? slug, ActorId? actorId = null)
  {
    if (Slug?.Equals(slug) != true)
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
    foreach (Language language in _statuses.Keys)
    {
      Unpublish(language, actorId);
    }
  }
  public void Unpublish(Language language, ActorId? actorId = null)
  {
    if (!_statuses.TryGetValue(language, out ContentStatus existingStatus))
    {
      throw new KitchenLocaleNotFoundException(this, language);
    }
    else if (existingStatus != ContentStatus.Unpublished)
    {
      Raise(new KitchenLocaleUnpublished(language), actorId);
    }
  }
  protected virtual void Handle(KitchenLocaleUnpublished @event)
  {
    _statuses[@event.Language] = ContentStatus.Unpublished;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Validation;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : AggregateRoot
{
  public new KitchenId Id => new(base.Id);
  public Guid EntityId => Id.ToGuid();

  public UserId OwnerId { get; private set; }

  public Confidentiality Confidentiality { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");

  public Slug? Slug { get; private set; }

  // TODO(fpion): Localization /w Publishing
  private readonly Dictionary<Language, KitchenLocale> _locales = [];

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

  public bool HasLocale(Language language) => _locales.ContainsKey(language);

  public void Publish(Language language, ActorId? actorId = null)
  {
    // TODO(fpion): implement
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

  public void Unpublish(Language language, ActorId? actorId = null)
  {
    // TODO(fpion): implement
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

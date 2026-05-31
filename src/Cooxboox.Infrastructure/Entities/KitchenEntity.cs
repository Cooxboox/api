using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenEntity : AggregateEntity
{
  public int KitchenId { get; private set; }
  public Guid UniqueId { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;

  public Confidentiality Confidentiality { get; private set; }

  public string Name { get; private set; } = string.Empty;

  public string? Slug { get; private set; }

  public List<KitchenLocaleEntity> Locales { get; private set; } = [];
  public List<PublishedKitchenLocaleEntity> PublishedLocales { get; private set; } = [];

  public KitchenEntity(KitchenCreated @event) : base(@event)
  {
    UniqueId = new KitchenId(@event.StreamId).EntityId;

    OwnerId = @event.OwnerId.Value;

    Name = @event.Name.Value;
  }

  private KitchenEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    foreach (KitchenLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void PublishLocale(KitchenLocalePublished @event)
  {
    base.Update(@event);

    KitchenLocaleEntity locale = FindLocale(@event.Language) ?? throw new InvalidOperationException($"The kitchen (KitchenId={KitchenId}) locale (Language={@event.Language}) was not found.");
    locale.Publish(@event);
  }

  public KitchenLocaleEntity? RemoveLocale(KitchenLocaleChanged @event)
  {
    base.Update(@event);

    return FindLocale(@event.Language);
  }

  public void Rename(KitchenRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public void SetConfidentiality(KitchenConfidentialityChanged @event)
  {
    base.Update(@event);

    Confidentiality = @event.Confidentiality;
  }

  public void SetLocale(KitchenLocaleChanged @event)
  {
    base.Update(@event);

    KitchenLocaleEntity? locale = FindLocale(@event.Language);
    if (locale is null)
    {
      locale = new KitchenLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void SetSlug(KitchenSlugChanged @event)
  {
    base.Update(@event);

    Slug = @event.Slug?.Value;
  }

  public PublishedKitchenLocaleEntity? UnpublishLocale(KitchenLocaleUnpublished @event)
  {
    base.Update(@event);

    KitchenLocaleEntity locale = FindLocale(@event.Language) ?? throw new InvalidOperationException($"The kitchen (KitchenId={KitchenId}) locale (Language={@event.Language}) was not found.");
    locale.Unpublish(@event);
    return locale.PublishedLocale;
  }

  private KitchenLocaleEntity? FindLocale(Language language) => Locales.SingleOrDefault(x => x.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()} (KitchenId={KitchenId})";
}

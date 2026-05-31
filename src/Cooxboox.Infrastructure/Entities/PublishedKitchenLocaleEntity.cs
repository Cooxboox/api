using Cooxboox.Core.Kitchens.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class PublishedKitchenLocaleEntity
{
  public KitchenLocaleEntity? KitchenLocale { get; private set; }
  public int KitchenLocaleId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public string? MetaDescription { get; private set; }
  public string? HtmlContent { get; private set; }

  public long Version { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime PublishedOn { get; private set; }

  public PublishedKitchenLocaleEntity? PublishedKitchenLocale { get; private set; }

  public PublishedKitchenLocaleEntity(KitchenLocaleEntity locale, KitchenLocalePublished @event)
  {
    KitchenLocale = locale;
    KitchenLocaleId = locale.KitchenLocaleId;

    Kitchen = locale.Kitchen;
    KitchenId = locale.KitchenId;
    Language = @event.Language.Code;

    Update(locale, @event);
  }

  private PublishedKitchenLocaleEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds() => PublishedBy is null ? [] : [new ActorId(PublishedBy)];

  public void Update(KitchenLocaleEntity locale, KitchenLocalePublished @event)
  {
    MetaDescription = locale.MetaDescription;
    HtmlContent = locale.HtmlContent;

    Version = @event.Version;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is PublishedKitchenLocaleEntity publishedLocale && publishedLocale.KitchenLocaleId == KitchenLocaleId;
  public override int GetHashCode() => KitchenLocaleId.GetHashCode();
  public override string ToString() => $"{base.ToString()} (KitchenLocaleId={KitchenLocaleId})";
}

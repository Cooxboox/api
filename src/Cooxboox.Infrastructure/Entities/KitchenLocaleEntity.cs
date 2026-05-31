using Cooxboox.Core.Kitchens.Events;
using Logitar;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenLocaleEntity
{
  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public string? HtmlContent { get; private set; }
  public string? MetaDescription { get; private set; }

  public long Version { get; private set; }
  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public KitchenLocaleEntity(KitchenEntity kitchen, KitchenLocaleChanged @event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    Language = @event.Language.Code;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private KitchenLocaleEntity()
  {
  }

  public void Update(KitchenLocaleChanged @event)
  {
    HtmlContent = @event.Locale.HtmlContent?.Value;
    MetaDescription = @event.Locale.MetaDescription?.Value;

    Version = @event.Version;
    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is KitchenLocaleEntity locale && locale.KitchenId == KitchenId && locale.Language == Language;
  public override int GetHashCode() => HashCode.Combine(KitchenId, Language);
  public override string ToString() => $"{base.ToString()} (KitchenId={KitchenId}, Language={Language})";
}

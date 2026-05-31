using Cooxboox.Core.Kitchens.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenLocaleEntity
{
  public int KitchenLocaleId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public string? MetaDescription { get; private set; }
  public string? HtmlContent { get; private set; }

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

  public virtual IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 2);
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    return actorIds;
  }

  public void Update(KitchenLocaleChanged @event)
  {
    MetaDescription = @event.Locale.MetaDescription?.Value;
    HtmlContent = @event.Locale.HtmlContent?.Value;

    Version = @event.Version;
    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is KitchenLocaleEntity locale && locale.KitchenLocaleId == KitchenLocaleId;
  public override int GetHashCode() => KitchenLocaleId.GetHashCode();
  public override string ToString() => $"{base.ToString()} (KitchenLocaleId={KitchenLocaleId})";
}

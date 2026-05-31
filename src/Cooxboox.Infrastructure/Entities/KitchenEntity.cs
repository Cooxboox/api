using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenEntity : AggregateEntity
{
  public int KitchenId { get; private set; }
  public Guid UniqueId { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;

  public Confidentiality Confidentiality { get; private set; }

  public string Name { get; private set; } = string.Empty;

  public string? Slug { get; private set; }

  public KitchenEntity(KitchenCreated @event) : base(@event)
  {
    UniqueId = new KitchenId(@event.StreamId).EntityId;

    OwnerId = @event.OwnerId.Value;

    Name = @event.Name.Value;
  }

  private KitchenEntity() : base()
  {
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

  public void SetSlug(KitchenSlugChanged @event)
  {
    base.Update(@event);

    Slug = @event.Slug?.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()} (KitchenId={KitchenId})";
}

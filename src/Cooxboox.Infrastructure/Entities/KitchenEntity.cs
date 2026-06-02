using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;
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
    actorIds.Add(new ActorId(OwnerId));
    return actorIds;
  }

  public void Rename(KitchenRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

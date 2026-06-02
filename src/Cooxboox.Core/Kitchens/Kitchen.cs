using Cooxboox.Core.Kitchens.Events;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : AggregateRoot
{
  public const string EntityKind = "Kitchen";

  public new KitchenId Id => new(base.Id);

  public Entity Entity => new(EntityKind, Id.EntityId);

  public UserId OwnerId { get; private set; }

  public Confidentiality Confidentiality { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");

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

  public override string ToString() => $"{Name} | {base.ToString()}";
}

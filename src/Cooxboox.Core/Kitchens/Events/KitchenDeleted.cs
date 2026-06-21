namespace Cooxboox.Core.Kitchens.Events;

public class KitchenDeleted : ChangeEvent
{
  public KitchenDeleted()
  {
  }

  public KitchenDeleted(Kitchen kitchen, Guid? userId = null)
  {
    EntityKind = Kitchen.EntityKind;
    EntityId = kitchen.EntityId;

    Version = kitchen.Version + 1;
    UserId = userId;
  }
}

namespace Cooxboox.Core.Kitchens.Events;

public class KitchenDeleted : DeleteEvent
{
  public KitchenDeleted() : base()
  {
  }

  public KitchenDeleted(Kitchen kitchen, Guid? userId = null) : base(kitchen, userId)
  {
  }
}

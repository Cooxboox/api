namespace Cooxboox.Core.Kitchens.Events;

public class KitchenUpdated : UpdateEvent
{
  public KitchenUpdated() : base()
  {
  }

  public KitchenUpdated(Kitchen kitchen) : base(kitchen)
  {
  }
}

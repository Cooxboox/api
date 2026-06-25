namespace Cooxboox.Core.Kitchens.Events;

public class KitchenUpdated : UpdateEvent
{
  public Change<Confidentiality>? Confidentiality { get; set; }

  public Change<string>? Name { get; set; }
  public Change<string>? Slug { get; set; }
  public Change<string>? Notes { get; set; }

  public KitchenUpdated() : base()
  {
  }

  public KitchenUpdated(Kitchen kitchen) : base(kitchen)
  {
  }
}

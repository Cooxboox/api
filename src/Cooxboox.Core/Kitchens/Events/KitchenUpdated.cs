namespace Cooxboox.Core.Kitchens.Events;

public class KitchenUpdated : ChangeEvent
{
  public Change<string>? Name { get; set; }
  public Change<string>? Slug { get; set; }
  public Change<string>? Notes { get; set; }

  public KitchenUpdated()
  {
  }

  public KitchenUpdated(Kitchen kitchen)
  {
    EntityKind = Kitchen.EntityKind;
    EntityId = kitchen.EntityId;

    Version = kitchen.Version;
    OccurredOn = kitchen.UpdatedOn;
    UserId = kitchen.UpdatedBy;
  }
}

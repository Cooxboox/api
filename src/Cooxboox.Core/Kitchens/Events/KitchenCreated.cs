namespace Cooxboox.Core.Kitchens.Events;

public class KitchenCreated : ChangeEvent
{
  public Guid OwnerId { get; set; }
  public Confidentiality Confidentiality { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? Notes { get; set; }

  public KitchenCreated()
  {
  }

  public KitchenCreated(Kitchen kitchen)
  {
    EntityKind = Kitchen.EntityKind;
    EntityId = kitchen.EntityId;
    Version = 1;
    OccurredOn = kitchen.CreatedOn;
    UserId = kitchen.CreatedBy;
    OwnerId = kitchen.OwnerId;
    Confidentiality = kitchen.Confidentiality;
    Name = kitchen.Name;
    Slug = kitchen.Slug;
    Notes = kitchen.Notes;
  }
}

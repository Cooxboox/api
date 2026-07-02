namespace Cooxboox.Core.Kitchens.Events;

public class KitchenCreated : CreateEvent
{
  public Guid OwnerId { get; set; }
  public Confidentiality Confidentiality { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? Notes { get; set; }

  public KitchenCreated() : base()
  {
  }

  public KitchenCreated(Kitchen kitchen) : base(kitchen)
  {
    OwnerId = kitchen.OwnerId;
    Confidentiality = kitchen.Confidentiality;

    Name = kitchen.Name;
    Slug = kitchen.Slug;
    Notes = kitchen.Notes;
  }
}

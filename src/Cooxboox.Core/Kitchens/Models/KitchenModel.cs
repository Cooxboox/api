using Krakenar.Contracts;
using Krakenar.Contracts.Actors;

namespace Cooxboox.Core.Kitchens.Models;

public class KitchenModel : Aggregate, IPublishableModel
{
  public Actor Owner { get; set; } = new();
  public Confidentiality Confidentiality { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? Notes { get; set; }

  public ContentStatus Status { get; set; }
  public Actor? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

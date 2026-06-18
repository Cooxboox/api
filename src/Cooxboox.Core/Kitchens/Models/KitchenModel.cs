using Krakenar.Contracts;
using Krakenar.Contracts.Actors;

namespace Cooxboox.Core.Kitchens.Models;

public class KitchenModel : Aggregate
{
  public Actor Owner { get; set; } = new();
  public Confidentiality Confidentiality { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? Notes { get; set; }
}

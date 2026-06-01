using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Kitchens.Models;

public record SearchKitchensPayload : SearchPayload
{
  public Confidentiality? Confidentiality { get; set; }

  public new List<KitchenSortOption> Sort { get; set; } = [];
}

using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientTypes.Models;

public record SearchIngredientTypesPayload : SearchPayload
{
  public new List<IngredientTypeSortOption> Sort { get; set; } = [];
}

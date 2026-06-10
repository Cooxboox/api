using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeTypes.Models;

public record SearchRecipeTypesPayload : SearchPayload
{
  public new List<RecipeTypeSortOption> Sort { get; set; } = [];
}

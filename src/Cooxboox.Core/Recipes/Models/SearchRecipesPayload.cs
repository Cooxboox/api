using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Recipes.Models;

public record SearchRecipesPayload : SearchPayload
{
  public new List<RecipeSortOption> Sort { get; set; } = [];
}

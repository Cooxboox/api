using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Recipes.Models;

public record SearchRecipesPayload : SearchPayload
{
  public Guid? RecipeTypeId { get; set; }

  public new List<RecipeSortOption> Sort { get; set; } = [];
}

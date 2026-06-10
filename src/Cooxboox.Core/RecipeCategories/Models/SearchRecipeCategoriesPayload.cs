using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeCategories.Models;

public record SearchRecipeCategoriesPayload : SearchPayload
{
  public new List<RecipeCategorySortOption> Sort { get; set; } = [];
}

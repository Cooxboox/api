using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientCategories.Models;

public record SearchIngredientCategoriesPayload : SearchPayload
{
  public new List<IngredientCategorySortOption> Sort { get; set; } = [];
}

using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Ingredients.Models;

public record SearchIngredientsPayload : SearchPayload
{
  public new List<IngredientSortOption> Sort { get; set; } = [];
}

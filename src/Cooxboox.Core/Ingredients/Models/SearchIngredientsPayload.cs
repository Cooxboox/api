using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Ingredients.Models;

public record SearchIngredientsPayload : SearchPayload
{
  public Guid? IngredientTypeId { get; set; }

  public new List<IngredientSortOption> Sort { get; set; } = [];
}

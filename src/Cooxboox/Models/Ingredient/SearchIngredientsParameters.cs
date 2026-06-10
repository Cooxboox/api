using Cooxboox.Core.Ingredients.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.Ingredient;

public record SearchIngredientsParameters : SearchParameters
{
  public virtual SearchIngredientsPayload ToPayload()
  {
    SearchIngredientsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out IngredientSort field))
      {
        payload.Sort.Add(new IngredientSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

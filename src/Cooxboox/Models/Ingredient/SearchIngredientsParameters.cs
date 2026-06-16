using Cooxboox.Core.Ingredients.Models;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Models.Ingredient;

public record SearchIngredientsParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public Guid? IngredientTypeId { get; set; }

  public virtual SearchIngredientsPayload ToPayload()
  {
    SearchIngredientsPayload payload = new()
    {
      IngredientTypeId = IngredientTypeId
    };
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

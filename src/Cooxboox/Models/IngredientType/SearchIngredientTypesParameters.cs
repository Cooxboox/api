using Cooxboox.Core.IngredientTypes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.IngredientType;

public record SearchIngredientTypesParameters : SearchParameters
{
  public virtual SearchIngredientTypesPayload ToPayload()
  {
    SearchIngredientTypesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out IngredientTypeSort field))
      {
        payload.Sort.Add(new IngredientTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

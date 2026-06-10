using Cooxboox.Core.RecipeTypes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.RecipeType;

public record SearchRecipeTypesParameters : SearchParameters
{
  public virtual SearchRecipeTypesPayload ToPayload()
  {
    SearchRecipeTypesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out RecipeTypeSort field))
      {
        payload.Sort.Add(new RecipeTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

using Cooxboox.Core.IngredientCategories.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.IngredientCategory;

public record SearchIngredientCategoriesParameters : SearchParameters
{
  public virtual SearchIngredientCategoriesPayload ToPayload()
  {
    SearchIngredientCategoriesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out IngredientCategorySort field))
      {
        payload.Sort.Add(new IngredientCategorySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

using Cooxboox.Core.RecipeCategories.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.RecipeCategory;

public record SearchRecipeCategoriesParameters : SearchParameters
{
  public virtual SearchRecipeCategoriesPayload ToPayload()
  {
    SearchRecipeCategoriesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out RecipeCategorySort field))
      {
        payload.Sort.Add(new RecipeCategorySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

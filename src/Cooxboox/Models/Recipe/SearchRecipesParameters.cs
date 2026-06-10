using Cooxboox.Core.Recipes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Models.Recipe;

public record SearchRecipesParameters : SearchParameters
{
  public virtual SearchRecipesPayload ToPayload()
  {
    SearchRecipesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in payload.Sort)
    {
      if (Enum.TryParse(sort.Field, out RecipeSort field))
      {
        payload.Sort.Add(new RecipeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}

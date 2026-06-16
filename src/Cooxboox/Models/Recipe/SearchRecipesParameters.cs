using Cooxboox.Core.Recipes.Models;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Models.Recipe;

public record SearchRecipesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public Guid? RecipeTypeId { get; set; }

  public virtual SearchRecipesPayload ToPayload()
  {
    SearchRecipesPayload payload = new()
    {
      RecipeTypeId = RecipeTypeId
    };
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

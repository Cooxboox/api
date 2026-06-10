using Cooxboox.Core.Recipes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Recipes;

public interface IRecipeQuerier
{
  Task<RecipeModel> ReadAsync(Recipe recipe, CancellationToken cancellationToken = default);
  Task<RecipeModel?> ReadAsync(RecipeId id, CancellationToken cancellationToken = default);
  Task<RecipeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<RecipeModel>> SearchAsync(SearchRecipesPayload payload, CancellationToken cancellationToken = default);
}

using Cooxboox.Core.Recipes.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.Recipes.Queries;

internal record SearchRecipesQuery(SearchRecipesPayload Payload) : IQuery<SearchResults<RecipeModel>>;

internal class SearchRecipesQueryHandler : IQueryHandler<SearchRecipesQuery, SearchResults<RecipeModel>>
{
  private readonly IRecipeQuerier _recipeQuerier;

  public SearchRecipesQueryHandler(IRecipeQuerier recipeQuerier)
  {
    _recipeQuerier = recipeQuerier;
  }

  public async Task<SearchResults<RecipeModel>> HandleAsync(SearchRecipesQuery query, CancellationToken cancellationToken)
  {
    return await _recipeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

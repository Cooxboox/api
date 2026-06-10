using Cooxboox.Core.RecipeTypes.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeTypes.Queries;

internal record SearchRecipeTypesQuery(SearchRecipeTypesPayload Payload) : IQuery<SearchResults<RecipeTypeModel>>;

internal class SearchRecipeTypesQueryHandler : IQueryHandler<SearchRecipeTypesQuery, SearchResults<RecipeTypeModel>>
{
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;

  public SearchRecipeTypesQueryHandler(IRecipeTypeQuerier recipeTypeQuerier)
  {
    _recipeTypeQuerier = recipeTypeQuerier;
  }

  public async Task<SearchResults<RecipeTypeModel>> HandleAsync(SearchRecipeTypesQuery query, CancellationToken cancellationToken)
  {
    return await _recipeTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

using Cooxboox.Core.IngredientTypes.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientTypes.Queries;

internal record SearchIngredientTypesQuery(SearchIngredientTypesPayload Payload) : IQuery<SearchResults<IngredientTypeModel>>;

internal class SearchIngredientTypesQueryHandler : IQueryHandler<SearchIngredientTypesQuery, SearchResults<IngredientTypeModel>>
{
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;

  public SearchIngredientTypesQueryHandler(IIngredientTypeQuerier ingredientTypeQuerier)
  {
    _ingredientTypeQuerier = ingredientTypeQuerier;
  }

  public async Task<SearchResults<IngredientTypeModel>> HandleAsync(SearchIngredientTypesQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

using Cooxboox.Core.Ingredients.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.Ingredients.Queries;

internal record SearchIngredientsQuery(SearchIngredientsPayload Payload) : IQuery<SearchResults<IngredientModel>>;

internal class SearchIngredientsQueryHandler : IQueryHandler<SearchIngredientsQuery, SearchResults<IngredientModel>>
{
  private readonly IIngredientQuerier _ingredientQuerier;

  public SearchIngredientsQueryHandler(IIngredientQuerier ingredientQuerier)
  {
    _ingredientQuerier = ingredientQuerier;
  }

  public async Task<SearchResults<IngredientModel>> HandleAsync(SearchIngredientsQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

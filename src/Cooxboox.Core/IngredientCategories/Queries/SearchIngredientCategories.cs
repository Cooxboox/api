using Cooxboox.Core.IngredientCategories.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientCategories.Queries;

internal record SearchIngredientCategoriesQuery(SearchIngredientCategoriesPayload Payload) : IQuery<SearchResults<IngredientCategoryModel>>;

internal class SearchIngredientCategoriesQueryHandler : IQueryHandler<SearchIngredientCategoriesQuery, SearchResults<IngredientCategoryModel>>
{
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;

  public SearchIngredientCategoriesQueryHandler(IIngredientCategoryQuerier ingredientCategoryQuerier)
  {
    _ingredientCategoryQuerier = ingredientCategoryQuerier;
  }

  public async Task<SearchResults<IngredientCategoryModel>> HandleAsync(SearchIngredientCategoriesQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientCategoryQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

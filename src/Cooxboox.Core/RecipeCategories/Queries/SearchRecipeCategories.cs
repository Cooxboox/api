using Cooxboox.Core.RecipeCategories.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeCategories.Queries;

internal record SearchRecipeCategoriesQuery(SearchRecipeCategoriesPayload Payload) : IQuery<SearchResults<RecipeCategoryModel>>;

internal class SearchRecipeCategoriesQueryHandler : IQueryHandler<SearchRecipeCategoriesQuery, SearchResults<RecipeCategoryModel>>
{
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;

  public SearchRecipeCategoriesQueryHandler(IRecipeCategoryQuerier recipeCategoryQuerier)
  {
    _recipeCategoryQuerier = recipeCategoryQuerier;
  }

  public async Task<SearchResults<RecipeCategoryModel>> HandleAsync(SearchRecipeCategoriesQuery query, CancellationToken cancellationToken)
  {
    return await _recipeCategoryQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

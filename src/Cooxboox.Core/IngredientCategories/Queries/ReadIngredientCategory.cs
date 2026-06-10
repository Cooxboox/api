using Cooxboox.Core.IngredientCategories.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientCategories.Queries;

internal record ReadIngredientCategoryQuery(Guid Id) : IQuery<IngredientCategoryModel?>;

internal class ReadIngredientCategoryQueryHandler : IQueryHandler<ReadIngredientCategoryQuery, IngredientCategoryModel?>
{
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;

  public ReadIngredientCategoryQueryHandler(IIngredientCategoryQuerier ingredientCategoryQuerier)
  {
    _ingredientCategoryQuerier = ingredientCategoryQuerier;
  }

  public async Task<IngredientCategoryModel?> HandleAsync(ReadIngredientCategoryQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientCategoryQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

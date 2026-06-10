using Cooxboox.Core.IngredientCategories.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientCategories;

public interface IIngredientCategoryQuerier
{
  Task<IngredientCategoryModel> ReadAsync(IngredientCategory ingredientCategory, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> ReadAsync(IngredientCategoryId id, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<IngredientCategoryModel>> SearchAsync(SearchIngredientCategoriesPayload payload, CancellationToken cancellationToken = default);
}

using Cooxboox.Core.RecipeCategories.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeCategories;

public interface IRecipeCategoryQuerier
{
  Task<RecipeCategoryModel> ReadAsync(RecipeCategory recipeCategory, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> ReadAsync(RecipeCategoryId id, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<RecipeCategoryModel>> SearchAsync(SearchRecipeCategoriesPayload payload, CancellationToken cancellationToken = default);
}

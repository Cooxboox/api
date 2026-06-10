namespace Cooxboox.Core.RecipeCategories;

public interface IRecipeCategoryRepository
{
  Task<RecipeCategory?> LoadAsync(RecipeCategoryId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<RecipeCategory>> LoadAsync(IEnumerable<RecipeCategoryId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(RecipeCategory recipeCategory, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RecipeCategory> recipeCategories, CancellationToken cancellationToken = default);
}

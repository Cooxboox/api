namespace Cooxboox.Core.IngredientCategories;

public interface IIngredientCategoryRepository
{
  Task<IngredientCategory?> LoadAsync(IngredientCategoryId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<IngredientCategory>> LoadAsync(IEnumerable<IngredientCategoryId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(IngredientCategory ingredientCategory, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<IngredientCategory> ingredientCategories, CancellationToken cancellationToken = default);
}

namespace Cooxboox.Core.Recipes;

public interface IRecipeRepository
{
  Task<Recipe?> LoadAsync(RecipeId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Recipe>> LoadAsync(IEnumerable<RecipeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Recipe recipe, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Recipe> recipes, CancellationToken cancellationToken = default);
}

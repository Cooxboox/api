namespace Cooxboox.Core.RecipeTypes;

public interface IRecipeTypeRepository
{
  Task<RecipeType?> LoadAsync(RecipeTypeId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<RecipeType>> LoadAsync(IEnumerable<RecipeTypeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(RecipeType recipeType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<RecipeType> recipeTypes, CancellationToken cancellationToken = default);
}

namespace Cooxboox.Core.Ingredients;

public interface IIngredientRepository
{
  Task<Ingredient?> LoadAsync(IngredientId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Ingredient>> LoadAsync(IEnumerable<IngredientId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Ingredient> ingredients, CancellationToken cancellationToken = default);
}

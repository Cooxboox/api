namespace Cooxboox.Core.IngredientTypes;

public interface IIngredientTypeRepository
{
  Task<IngredientType?> LoadAsync(IngredientTypeId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<IngredientType>> LoadAsync(IEnumerable<IngredientTypeId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(IngredientType ingredientType, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<IngredientType> ingredientTypes, CancellationToken cancellationToken = default);
}

using Cooxboox.Core.IngredientTypes;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class IngredientTypeRepository : Repository, IIngredientTypeRepository
{
  public IngredientTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<IngredientType?> LoadAsync(IngredientTypeId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<IngredientType>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<IngredientType>> LoadAsync(IEnumerable<IngredientTypeId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<IngredientType>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(IngredientType ingredientType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredientType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<IngredientType> ingredientTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredientTypes, cancellationToken);
  }
}

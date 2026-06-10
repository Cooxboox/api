using Cooxboox.Core.Ingredients;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class IngredientRepository : Repository, IIngredientRepository
{
  public IngredientRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Ingredient?> LoadAsync(IngredientId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Ingredient>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Ingredient>> LoadAsync(IEnumerable<IngredientId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Ingredient>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Ingredient ingredient, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredient, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Ingredient> ingredients, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredients, cancellationToken);
  }
}

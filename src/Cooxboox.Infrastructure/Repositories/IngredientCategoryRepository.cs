using Cooxboox.Core.IngredientCategories;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class IngredientCategoryRepository : Repository, IIngredientCategoryRepository
{
  public IngredientCategoryRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<IngredientCategory?> LoadAsync(IngredientCategoryId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<IngredientCategory>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<IngredientCategory>> LoadAsync(IEnumerable<IngredientCategoryId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<IngredientCategory>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(IngredientCategory ingredientCategory, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredientCategory, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<IngredientCategory> ingredientCategories, CancellationToken cancellationToken)
  {
    await base.SaveAsync(ingredientCategories, cancellationToken);
  }
}

using Cooxboox.Core.RecipeTypes;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class RecipeTypeRepository : Repository, IRecipeTypeRepository
{
  public RecipeTypeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<RecipeType?> LoadAsync(RecipeTypeId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<RecipeType>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<RecipeType>> LoadAsync(IEnumerable<RecipeTypeId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<RecipeType>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(RecipeType recipeType, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipeType, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<RecipeType> recipeTypes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipeTypes, cancellationToken);
  }
}

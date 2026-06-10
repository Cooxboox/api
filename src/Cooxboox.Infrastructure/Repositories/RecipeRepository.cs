using Cooxboox.Core.Recipes;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class RecipeRepository : Repository, IRecipeRepository
{
  public RecipeRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Recipe?> LoadAsync(RecipeId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Recipe>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Recipe>> LoadAsync(IEnumerable<RecipeId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Recipe>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Recipe recipe, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipe, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Recipe> recipes, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipes, cancellationToken);
  }
}

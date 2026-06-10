using Cooxboox.Core.RecipeCategories;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class RecipeCategoryRepository : Repository, IRecipeCategoryRepository
{
  public RecipeCategoryRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<RecipeCategory?> LoadAsync(RecipeCategoryId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<RecipeCategory>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<RecipeCategory>> LoadAsync(IEnumerable<RecipeCategoryId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<RecipeCategory>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(RecipeCategory recipeCategory, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipeCategory, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<RecipeCategory> recipeCategories, CancellationToken cancellationToken)
  {
    await base.SaveAsync(recipeCategories, cancellationToken);
  }
}

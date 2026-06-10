using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class RecipeCategoryQuerier : IRecipeCategoryQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<RecipeCategoryEntity> _recipeCategories;
  private readonly ISqlHelper _sqlHelper;

  public RecipeCategoryQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _recipeCategories = cooxboox.RecipeCategories;
    _sqlHelper = sqlHelper;
  }

  public async Task<RecipeCategoryModel> ReadAsync(RecipeCategory recipeCategory, CancellationToken cancellationToken)
  {
    return await ReadAsync(recipeCategory.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The recipe category entity 'StreamId={recipeCategory.Id}' was not found.");
  }
  public async Task<RecipeCategoryModel?> ReadAsync(RecipeCategoryId id, CancellationToken cancellationToken)
  {
    RecipeCategoryEntity? recipeCategory = await _recipeCategories.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipeCategory is null ? null : await MapAsync(recipeCategory, cancellationToken);
  }
  public async Task<RecipeCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeCategoryEntity? recipeCategory = await _recipeCategories.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipeCategory is null ? null : await MapAsync(recipeCategory, cancellationToken);
  }

  public async Task<SearchResults<RecipeCategoryModel>> SearchAsync(SearchRecipeCategoriesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.RecipeCategories.Table).SelectAll(Db.RecipeCategories.Table)
      .ApplyIdFilter(Db.RecipeCategories.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.RecipeCategories.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.RecipeCategories.Name);

    IQueryable<RecipeCategoryEntity> query = _recipeCategories.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RecipeCategoryEntity>? ordered = null;
    foreach (RecipeCategorySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RecipeCategorySort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RecipeCategorySort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case RecipeCategorySort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RecipeCategoryEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RecipeCategoryModel> recipeCategories = await MapAsync(entities, cancellationToken);

    return new SearchResults<RecipeCategoryModel>(recipeCategories, total);
  }

  private async Task<RecipeCategoryModel> MapAsync(RecipeCategoryEntity recipeCategory, CancellationToken cancellationToken)
  {
    return (await MapAsync([recipeCategory], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RecipeCategoryModel>> MapAsync(IEnumerable<RecipeCategoryEntity> recipeCategories, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = recipeCategories.SelectMany(recipeCategory => recipeCategory.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return recipeCategories.Select(mapper.ToRecipeCategory).ToList().AsReadOnly();
  }
}

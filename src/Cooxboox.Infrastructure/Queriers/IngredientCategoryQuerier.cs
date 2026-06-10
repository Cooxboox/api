using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class IngredientCategoryQuerier : IIngredientCategoryQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<IngredientCategoryEntity> _ingredientCategories;
  private readonly ISqlHelper _sqlHelper;

  public IngredientCategoryQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _ingredientCategories = cooxboox.IngredientCategories;
    _sqlHelper = sqlHelper;
  }

  public async Task<IngredientCategoryModel> ReadAsync(IngredientCategory ingredientCategory, CancellationToken cancellationToken)
  {
    return await ReadAsync(ingredientCategory.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The ingredient category entity 'StreamId={ingredientCategory.Id}' was not found.");
  }
  public async Task<IngredientCategoryModel?> ReadAsync(IngredientCategoryId id, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _ingredientCategories.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredientCategory is null ? null : await MapAsync(ingredientCategory, cancellationToken);
  }
  public async Task<IngredientCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _ingredientCategories.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredientCategory is null ? null : await MapAsync(ingredientCategory, cancellationToken);
  }

  public async Task<SearchResults<IngredientCategoryModel>> SearchAsync(SearchIngredientCategoriesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.IngredientCategories.Table).SelectAll(Db.IngredientCategories.Table)
      .ApplyIdFilter(Db.IngredientCategories.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.IngredientCategories.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.IngredientCategories.Name);

    IQueryable<IngredientCategoryEntity> query = _ingredientCategories.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<IngredientCategoryEntity>? ordered = null;
    foreach (IngredientCategorySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case IngredientCategorySort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case IngredientCategorySort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case IngredientCategorySort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    IngredientCategoryEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<IngredientCategoryModel> ingredientCategories = await MapAsync(entities, cancellationToken);

    return new SearchResults<IngredientCategoryModel>(ingredientCategories, total);
  }

  private async Task<IngredientCategoryModel> MapAsync(IngredientCategoryEntity ingredientCategory, CancellationToken cancellationToken)
  {
    return (await MapAsync([ingredientCategory], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<IngredientCategoryModel>> MapAsync(IEnumerable<IngredientCategoryEntity> ingredientCategories, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ingredientCategories.SelectMany(ingredientCategory => ingredientCategory.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return ingredientCategories.Select(mapper.ToIngredientCategory).ToList().AsReadOnly();
  }
}

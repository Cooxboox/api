using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class IngredientTypeQuerier : IIngredientTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<IngredientTypeEntity> _ingredientTypes;
  private readonly ISqlHelper _sqlHelper;

  public IngredientTypeQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _ingredientTypes = cooxboox.IngredientTypes;
    _sqlHelper = sqlHelper;
  }

  public async Task<IngredientTypeModel> ReadAsync(IngredientType ingredientType, CancellationToken cancellationToken)
  {
    return await ReadAsync(ingredientType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The ingredient type entity 'StreamId={ingredientType.Id}' was not found.");
  }
  public async Task<IngredientTypeModel?> ReadAsync(IngredientTypeId id, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _ingredientTypes.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredientType is null ? null : await MapAsync(ingredientType, cancellationToken);
  }
  public async Task<IngredientTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _ingredientTypes.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredientType is null ? null : await MapAsync(ingredientType, cancellationToken);
  }

  public async Task<SearchResults<IngredientTypeModel>> SearchAsync(SearchIngredientTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.IngredientTypes.Table).SelectAll(Db.IngredientTypes.Table)
      .ApplyIdFilter(Db.IngredientTypes.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.IngredientTypes.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.IngredientTypes.Name);

    IQueryable<IngredientTypeEntity> query = _ingredientTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<IngredientTypeEntity>? ordered = null;
    foreach (IngredientTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case IngredientTypeSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case IngredientTypeSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case IngredientTypeSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    IngredientTypeEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<IngredientTypeModel> ingredientTypes = await MapAsync(entities, cancellationToken);

    return new SearchResults<IngredientTypeModel>(ingredientTypes, total);
  }

  private async Task<IngredientTypeModel> MapAsync(IngredientTypeEntity ingredientType, CancellationToken cancellationToken)
  {
    return (await MapAsync([ingredientType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<IngredientTypeModel>> MapAsync(IEnumerable<IngredientTypeEntity> ingredientTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ingredientTypes.SelectMany(ingredientType => ingredientType.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return ingredientTypes.Select(mapper.ToIngredientType).ToList().AsReadOnly();
  }
}

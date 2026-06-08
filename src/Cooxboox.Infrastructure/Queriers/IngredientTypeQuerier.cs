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
  private readonly DbSet<IngredientTypeEntity> _ingredienttypes;
  private readonly ISqlHelper _sqlHelper;

  public IngredientTypeQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _ingredienttypes = cooxboox.IngredientTypes;
    _sqlHelper = sqlHelper;
  }

  public async Task<IngredientTypeModel> ReadAsync(IngredientType ingredienttype, CancellationToken cancellationToken)
  {
    return await ReadAsync(ingredienttype.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The ingredienttype entity 'StreamId={ingredienttype.Id}' was not found.");
  }
  public async Task<IngredientTypeModel?> ReadAsync(IngredientTypeId id, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredienttype = await _ingredienttypes.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredienttype is null ? null : await MapAsync(ingredienttype, cancellationToken);
  }
  public async Task<IngredientTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredienttype = await _ingredienttypes.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredienttype is null ? null : await MapAsync(ingredienttype, cancellationToken);
  }

  public async Task<SearchResults<IngredientTypeModel>> SearchAsync(SearchIngredientTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.IngredientTypes.Table).SelectAll(Db.IngredientTypes.Table)
      .ApplyIdFilter(Db.IngredientTypes.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.IngredientTypes.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.IngredientTypes.Name);

    IQueryable<IngredientTypeEntity> query = _ingredienttypes.FromQuery(builder).AsNoTracking();

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
    IReadOnlyCollection<IngredientTypeModel> ingredienttypes = await MapAsync(entities, cancellationToken);

    return new SearchResults<IngredientTypeModel>(ingredienttypes, total);
  }

  private async Task<IngredientTypeModel> MapAsync(IngredientTypeEntity ingredienttype, CancellationToken cancellationToken)
  {
    return (await MapAsync([ingredienttype], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<IngredientTypeModel>> MapAsync(IEnumerable<IngredientTypeEntity> ingredienttypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ingredienttypes.SelectMany(ingredienttype => ingredienttype.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return ingredienttypes.Select(mapper.ToIngredientType).ToList().AsReadOnly();
  }
}

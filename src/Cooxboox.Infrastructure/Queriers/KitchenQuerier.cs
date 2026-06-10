using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class KitchenQuerier : IKitchenQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<KitchenEntity> _kitchens;
  private readonly ISqlHelper _sqlHelper;

  public KitchenQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _kitchens = cooxboox.Kitchens;
    _sqlHelper = sqlHelper;
  }

  public async Task<int> CountAsync(CancellationToken cancellationToken)
  {
    return await _kitchens.CountAsync(x => x.OwnerId == _context.UserId.Value, cancellationToken);
  }

  public async Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    return await ReadAsync(kitchen.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={kitchen.Id}' was not found.");
  }
  public async Task<KitchenModel?> ReadAsync(KitchenId id, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _kitchens.AsNoTracking()
      .Include(x => x.Locales)
      .Where(x => x.StreamId == id.Value && x.OwnerId == _context.UserId.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return kitchen is null ? null : await MapAsync(kitchen, cancellationToken);
  }
  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _kitchens.AsNoTracking()
      .Include(x => x.Locales)
      .Where(x => x.EntityId == id && x.OwnerId == _context.UserId.Value)
      .SingleOrDefaultAsync(cancellationToken);
    return kitchen is null ? null : await MapAsync(kitchen, cancellationToken);
  }

  public async Task<SearchResults<KitchenModel>> SearchAsync(SearchKitchensPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Kitchens.Table).SelectAll(Db.Kitchens.Table)
      .Where(Db.Kitchens.OwnerId, Operators.IsEqualTo(_context.UserId.Value))
      .ApplyIdFilter(Db.Kitchens.EntityId, payload.Ids);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Kitchens.Name, Db.Kitchens.Slug);

    if (payload.Confidentiality.HasValue)
    {
      builder.Where(Db.Kitchens.Confidentiality, Operators.IsEqualTo(payload.Confidentiality.Value.ToString()));
    }

    IQueryable<KitchenEntity> query = _kitchens.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<KitchenEntity>? ordered = null;
    foreach (KitchenSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case KitchenSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case KitchenSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case KitchenSort.Slug:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Slug) : query.OrderBy(x => x.Slug))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Slug) : ordered.ThenBy(x => x.Slug));
          break;
        case KitchenSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    KitchenEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<KitchenModel> kitchens = await MapAsync(entities, cancellationToken);

    return new SearchResults<KitchenModel>(kitchens, total);
  }

  private async Task<KitchenModel> MapAsync(KitchenEntity kitchen, CancellationToken cancellationToken)
  {
    return (await MapAsync([kitchen], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<KitchenModel>> MapAsync(IEnumerable<KitchenEntity> kitchens, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = kitchens.SelectMany(kitchen => kitchen.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return kitchens.Select(mapper.ToKitchen).ToList().AsReadOnly();
  }
}

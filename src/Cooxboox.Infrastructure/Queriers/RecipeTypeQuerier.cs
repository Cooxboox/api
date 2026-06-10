using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class RecipeTypeQuerier : IRecipeTypeQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<RecipeTypeEntity> _recipeTypes;
  private readonly ISqlHelper _sqlHelper;

  public RecipeTypeQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _recipeTypes = cooxboox.RecipeTypes;
    _sqlHelper = sqlHelper;
  }

  public async Task<RecipeTypeModel> ReadAsync(RecipeType recipeType, CancellationToken cancellationToken)
  {
    return await ReadAsync(recipeType.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The recipe type entity 'StreamId={recipeType.Id}' was not found.");
  }
  public async Task<RecipeTypeModel?> ReadAsync(RecipeTypeId id, CancellationToken cancellationToken)
  {
    RecipeTypeEntity? recipeType = await _recipeTypes.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipeType is null ? null : await MapAsync(recipeType, cancellationToken);
  }
  public async Task<RecipeTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeTypeEntity? recipeType = await _recipeTypes.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipeType is null ? null : await MapAsync(recipeType, cancellationToken);
  }

  public async Task<SearchResults<RecipeTypeModel>> SearchAsync(SearchRecipeTypesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.RecipeTypes.Table).SelectAll(Db.RecipeTypes.Table)
      .ApplyIdFilter(Db.RecipeTypes.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.RecipeTypes.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.RecipeTypes.Name);

    IQueryable<RecipeTypeEntity> query = _recipeTypes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RecipeTypeEntity>? ordered = null;
    foreach (RecipeTypeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RecipeTypeSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RecipeTypeSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case RecipeTypeSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RecipeTypeEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RecipeTypeModel> recipeTypes = await MapAsync(entities, cancellationToken);

    return new SearchResults<RecipeTypeModel>(recipeTypes, total);
  }

  private async Task<RecipeTypeModel> MapAsync(RecipeTypeEntity recipeType, CancellationToken cancellationToken)
  {
    return (await MapAsync([recipeType], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RecipeTypeModel>> MapAsync(IEnumerable<RecipeTypeEntity> recipeTypes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = recipeTypes.SelectMany(recipeType => recipeType.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return recipeTypes.Select(mapper.ToRecipeType).ToList().AsReadOnly();
  }
}

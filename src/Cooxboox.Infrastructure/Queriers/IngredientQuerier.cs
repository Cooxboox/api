using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class IngredientQuerier : IIngredientQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<IngredientEntity> _ingredients;
  private readonly ISqlHelper _sqlHelper;

  public IngredientQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _ingredients = cooxboox.Ingredients;
    _sqlHelper = sqlHelper;
  }

  public async Task<IngredientModel> ReadAsync(Ingredient ingredient, CancellationToken cancellationToken)
  {
    return await ReadAsync(ingredient.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The ingredient entity 'StreamId={ingredient.Id}' was not found.");
  }
  public async Task<IngredientModel?> ReadAsync(IngredientId id, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _ingredients.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.IngredientType).ThenInclude(x => x!.Locales)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredient is null ? null : await MapAsync(ingredient, cancellationToken);
  }
  public async Task<IngredientModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _ingredients.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.IngredientType).ThenInclude(x => x!.Locales)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return ingredient is null ? null : await MapAsync(ingredient, cancellationToken);
  }

  public async Task<SearchResults<IngredientModel>> SearchAsync(SearchIngredientsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Ingredients.Table).SelectAll(Db.Ingredients.Table)
      .ApplyIdFilter(Db.Ingredients.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.Ingredients.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Ingredients.Name);

    if (payload.IngredientTypeId.HasValue)
    {
      OperatorCondition condition = new(Db.IngredientTypes.EntityId, Operators.IsEqualTo(payload.IngredientTypeId.Value));
      builder.Join(Db.IngredientTypes.IngredientTypeId, Db.Ingredients.IngredientTypeId, condition);
    }

    IQueryable<IngredientEntity> query = _ingredients.FromQuery(builder).AsNoTracking()
      .Include(x => x.IngredientType);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<IngredientEntity>? ordered = null;
    foreach (IngredientSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case IngredientSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case IngredientSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case IngredientSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    IngredientEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<IngredientModel> ingredients = await MapAsync(entities, cancellationToken);

    return new SearchResults<IngredientModel>(ingredients, total);
  }

  private async Task<IngredientModel> MapAsync(IngredientEntity ingredient, CancellationToken cancellationToken)
  {
    return (await MapAsync([ingredient], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<IngredientModel>> MapAsync(IEnumerable<IngredientEntity> ingredients, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = ingredients.SelectMany(ingredient => ingredient.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return ingredients.Select(mapper.ToIngredient).ToList().AsReadOnly();
  }
}

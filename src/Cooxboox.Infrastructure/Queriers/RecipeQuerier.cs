using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class RecipeQuerier : IRecipeQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<RecipeEntity> _recipes;
  private readonly ISqlHelper _sqlHelper;

  public RecipeQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox, ISqlHelper sqlHelper)
  {
    _actorService = actorService;
    _context = context;
    _recipes = cooxboox.Recipes;
    _sqlHelper = sqlHelper;
  }

  public async Task<RecipeModel> ReadAsync(Recipe recipe, CancellationToken cancellationToken)
  {
    return await ReadAsync(recipe.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The recipe type entity 'StreamId={recipe.Id}' was not found.");
  }
  public async Task<RecipeModel?> ReadAsync(RecipeId id, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _recipes.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipe is null ? null : await MapAsync(recipe, cancellationToken);
  }
  public async Task<RecipeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _recipes.AsNoTracking()
      .Where(x => x.EntityId == id && x.Kitchen!.StreamId == _context.KitchenId.Value)
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(cancellationToken);
    return recipe is null ? null : await MapAsync(recipe, cancellationToken);
  }

  public async Task<SearchResults<RecipeModel>> SearchAsync(SearchRecipesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sqlHelper.Query(Db.Recipes.Table).SelectAll(Db.Recipes.Table)
      .ApplyIdFilter(Db.Recipes.EntityId, payload.Ids)
      .ApplyKitchenFilter(Db.Recipes.KitchenId, _context.KitchenId);
    _sqlHelper.ApplyTextSearch(builder, payload.Search, Db.Recipes.Name);

    if (payload.RecipeTypeId.HasValue)
    {
      OperatorCondition condition = new(Db.RecipeTypes.EntityId, Operators.IsEqualTo(payload.RecipeTypeId.Value));
      builder.Join(Db.RecipeTypes.RecipeTypeId, Db.Recipes.RecipeTypeId, condition);
    }

    IQueryable<RecipeEntity> query = _recipes.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<RecipeEntity>? ordered = null;
    foreach (RecipeSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case RecipeSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case RecipeSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case RecipeSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    RecipeEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<RecipeModel> recipes = await MapAsync(entities, cancellationToken);

    return new SearchResults<RecipeModel>(recipes, total);
  }

  private async Task<RecipeModel> MapAsync(RecipeEntity recipe, CancellationToken cancellationToken)
  {
    return (await MapAsync([recipe], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<RecipeModel>> MapAsync(IEnumerable<RecipeEntity> recipes, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = recipes.SelectMany(recipe => recipe.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return recipes.Select(mapper.ToRecipe).ToList().AsReadOnly();
  }
}

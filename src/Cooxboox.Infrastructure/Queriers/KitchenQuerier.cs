using Cooxboox.Core;
using Cooxboox.Core.Actors;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Queriers;

internal class KitchenQuerier : IKitchenQuerier
{
  private readonly IActorService _actorService;
  private readonly IContext _context;
  private readonly DbSet<KitchenEntity> _kitchens;

  public KitchenQuerier(IActorService actorService, IContext context, CooxbooxContext cooxboox)
  {
    _actorService = actorService;
    _context = context;
    _kitchens = cooxboox.Kitchens;
  }

  public async Task<int> CountAsync(CancellationToken cancellationToken)
  {
    return await _kitchens.CountAsync(x => x.OwnerId == _context.UserId.Value, cancellationToken);
  }

  public async Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    return await ReadAsync(kitchen.Id, cancellationToken) ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={kitchen.Id}' was not found.");
  }
  public async Task<KitchenModel?> ReadAsync(KitchenId id, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _kitchens.AsNoTracking()
      .Where(x => x.StreamId == id.Value && x.OwnerId == _context.UserId.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return kitchen is null ? null : await MapAsync(kitchen, cancellationToken);
  }
  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _kitchens.AsNoTracking()
      .Where(x => x.UniqueId == id && x.OwnerId == _context.UserId.Value)
      .SingleOrDefaultAsync(cancellationToken);

    return kitchen is null ? null : await MapAsync(kitchen, cancellationToken);
  }

  private async Task<KitchenModel> MapAsync(KitchenEntity kitchen, CancellationToken cancellationToken)
  {
    return (await MapAsync([kitchen], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<KitchenModel>> MapAsync(IEnumerable<KitchenEntity> kitchens, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = kitchens.SelectMany(kitchen => kitchen.GetActorIds()).Distinct();
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return kitchens.Select(mapper.ToKitchen).ToList().AsReadOnly();
  }
}

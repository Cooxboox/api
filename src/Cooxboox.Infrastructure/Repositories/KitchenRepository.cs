using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Infrastructure.Actors;
using Krakenar.Contracts.Actors;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Repositories;

internal class KitchenRepository : Repository, IKitchenRepository
{
  private readonly IActorService _actorService;
  private readonly IContext _context;

  public KitchenRepository(IActorService actorService, IContext context, CooxbooxContext database) : base(database)
  {
    _actorService = actorService;
    _context = context;
  }

  public void Add(Kitchen kitchen)
  {
    Database.Kitchens.Add(kitchen);
    base.RecordChange(new KitchenCreated(kitchen));
  }
  public void Remove(Kitchen kitchen)
  {
    Database.Kitchens.Remove(kitchen);
    base.RecordChange(new KitchenDeleted(kitchen));
  }
  public void Update(Kitchen kitchen, KitchenUpdated record)
  {
    Database.Kitchens.Update(kitchen);
    base.RecordChange(record);
  }

  public async Task<int> CountAsync(CancellationToken cancellationToken)
  {
    return await Database.Kitchens.CountAsync(x => x.OwnerId == _context.UserId, cancellationToken);
  }

  public async Task EnsureUnicityAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    if (kitchen.Slug is not null)
    {
      Guid? kitchenId = await Database.Kitchens.Where(x => x.Slug == kitchen.Slug)
        .Select(x => (Guid?)x.Id)
        .SingleOrDefaultAsync(cancellationToken);
      if (kitchenId.HasValue && !kitchenId.Value.Equals(kitchen.Id))
      {
        throw new KitchenSlugAlreadyUsedException(kitchen, kitchenId.Value);
      }
    }
  }

  public async Task<Kitchen?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await Database.Kitchens.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
  }

  public async Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    return await ReadAsync(kitchen.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The kitchen 'Id={kitchen.Id}' was not found.");
  }
  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Kitchen? kitchen = await Database.Kitchens.AsNoTracking()
      .Where(x => x.Id == id && x.OwnerId == _context.UserId)
      .SingleOrDefaultAsync(cancellationToken);

    return kitchen is null ? null : await MapAsync(kitchen, cancellationToken);
  }

  private async Task<KitchenModel> MapAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    return (await MapAsync([kitchen], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<KitchenModel>> MapAsync(IEnumerable<Kitchen> kitchens, CancellationToken cancellationToken)
  {
    IEnumerable<Guid> userIds = kitchens.SelectMany(kitchen => kitchen.GetUserIds());
    IReadOnlyDictionary<Guid, Actor> actors = await _actorService.FindAsync(userIds, cancellationToken);
    Mapper mapper = new(actors);

    return kitchens.Select(mapper.ToKitchen).ToList().AsReadOnly();
  }
}

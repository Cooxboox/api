using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Kitchens.Models;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure.Repositories;

internal class KitchenRepository : IKitchenRepository
{
  private readonly IContext _context;
  private readonly CooxbooxContext _cooxboox;

  public KitchenRepository(IContext context, CooxbooxContext cooxboox)
  {
    _context = context;
    _cooxboox = cooxboox;
  }

  public void Add(Kitchen kitchen)
  {
    _cooxboox.Kitchens.Add(kitchen);

    KitchenCreated @event = new(kitchen);
    // TODO(fpion): event
  }
  public void Remove(Kitchen kitchen)
  {
    _cooxboox.Kitchens.Remove(kitchen);

    KitchenDeleted @event = new(kitchen);
    // TODO(fpion): event
  }
  public void Update(Kitchen kitchen, KitchenUpdated @event)
  {
    _cooxboox.Kitchens.Update(kitchen);

    // TODO(fpion): event
  }

  public async Task<int> CountAsync(CancellationToken cancellationToken)
  {
    return await _cooxboox.Kitchens.CountAsync(x => x.OwnerId == _context.UserId, cancellationToken);
  }

  public async Task EnsureUnicityAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    if (kitchen.Slug is not null)
    {
      Guid? kitchenId = await _cooxboox.Kitchens
        .Where(x => x.Slug == kitchen.Slug)
        .Select(x => (Guid?)x.EntityId)
        .SingleOrDefaultAsync(cancellationToken);
      if (kitchenId.HasValue && !kitchenId.Value.Equals(kitchen.EntityId))
      {
        throw new KitchenSlugAlreadyUsedException(kitchen, kitchenId.Value);
      }
    }
  }

  public async Task<Kitchen?> LoadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.EntityId == id, cancellationToken);
  }

  public async Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    return await ReadAsync(kitchen.EntityId, cancellationToken)
      ?? throw new InvalidOperationException($"The kitchen 'Id={kitchen.EntityId}' was not found.");
  }
  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Kitchen? kitchen = await _cooxboox.Kitchens.AsNoTracking()
      .Where(x => x.EntityId == id && x.OwnerId == _context.UserId)
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
    // TODO(fpion): find actors
    Mapper mapper = new(); // TODO(fpion): pass actors

    return kitchens.Select(mapper.ToKitchen).ToList().AsReadOnly();
  }
}

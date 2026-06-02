using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Repositories;

internal class KitchenRepository : Repository, IKitchenRepository
{
  public KitchenRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public async Task<Kitchen?> LoadAsync(KitchenId id, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Kitchen>(id.StreamId, isDeleted: false, cancellationToken);
  }
  public async Task<IReadOnlyCollection<Kitchen>?> LoadAsync(IEnumerable<KitchenId> ids, CancellationToken cancellationToken)
  {
    return await base.LoadAsync<Kitchen>(ids.Select(id => id.StreamId), isDeleted: false, cancellationToken);
  }

  public async Task SaveAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    await base.SaveAsync(kitchen, cancellationToken);
  }
  public async Task SaveAsync(IEnumerable<Kitchen> kitchens, CancellationToken cancellationToken)
  {
    await base.SaveAsync(kitchens, cancellationToken);
  }
}

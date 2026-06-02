namespace Cooxboox.Core.Kitchens;

public interface IKitchenRepository
{
  Task<Kitchen?> LoadAsync(KitchenId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Kitchen>?> LoadAsync(IEnumerable<KitchenId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Kitchen kitchen, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Kitchen> kitchens, CancellationToken cancellationToken = default);
}

using Cooxboox.Core.Kitchens.Models;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenRepository
{
  void Add(Kitchen kitchen);
  void Remove(Kitchen kitchen);
  void Update(Kitchen kitchen);

  Task<int> CountAsync(CancellationToken cancellationToken = default);

  Task EnsureUnicityAsync(Kitchen kitchen, CancellationToken cancellationToken = default);

  Task<Kitchen?> LoadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken = default);
  Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}

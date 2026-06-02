using Cooxboox.Core.Kitchens.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenQuerier
{
  Task<int> CountAsync(CancellationToken cancellationToken = default);

  Task<KitchenModel> ReadAsync(Kitchen kitchen, CancellationToken cancellationToken = default);
  Task<KitchenModel?> ReadAsync(KitchenId id, CancellationToken cancellationToken = default);
  Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<KitchenModel>> SearchAsync(SearchKitchensPayload payload, CancellationToken cancellationToken = default);
}

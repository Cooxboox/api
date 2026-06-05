using Cooxboox.Core.Kitchens.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Queries;

internal record ReadKitchenQuery(Guid Id) : IQuery<KitchenModel?>;

internal class ReadKitchenQueryHandler : IQueryHandler<ReadKitchenQuery, KitchenModel?>
{
  private readonly IKitchenQuerier _kitchenQuerier;

  public ReadKitchenQueryHandler(IKitchenQuerier kitchenQuerier)
  {
    _kitchenQuerier = kitchenQuerier;
  }

  public async Task<KitchenModel?> HandleAsync(ReadKitchenQuery query, CancellationToken cancellationToken)
  {
    return await _kitchenQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

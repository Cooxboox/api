using Cooxboox.Core.Kitchens.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Queries;

internal record ReadKitchenQuery(Guid Id) : IQuery<KitchenModel?>;

internal class ReadKitchenQueryHandler : IQueryHandler<ReadKitchenQuery, KitchenModel?>
{
  private readonly IKitchenRepository _kitchenRepository;

  public ReadKitchenQueryHandler(IKitchenRepository kitchenRepository)
  {
    _kitchenRepository = kitchenRepository;
  }

  public async Task<KitchenModel?> HandleAsync(ReadKitchenQuery query, CancellationToken cancellationToken)
  {
    return await _kitchenRepository.ReadAsync(query.Id, cancellationToken);
  }
}

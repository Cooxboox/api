using Cooxboox.Core.Kitchens.Models;
using Krakenar.Contracts.Search;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Queries;

internal record SearchKitchensQuery(SearchKitchensPayload Payload) : IQuery<SearchResults<KitchenModel>>;

internal class SearchKitchensQueryHandler : IQueryHandler<SearchKitchensQuery, SearchResults<KitchenModel>>
{
  private readonly IKitchenQuerier _kitchenQuerier;

  public SearchKitchensQueryHandler(IKitchenQuerier kitchenQuerier)
  {
    _kitchenQuerier = kitchenQuerier;
  }

  public async Task<SearchResults<KitchenModel>> HandleAsync(SearchKitchensQuery query, CancellationToken cancellationToken)
  {
    return await _kitchenQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}

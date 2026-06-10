using Cooxboox.Core.Caching;
using Cooxboox.Core.Identity;
using Krakenar.Client;
using Krakenar.Contracts.Realms;

namespace Cooxboox.Infrastructure.Identity;

internal class RealmGateway : IRealmGateway
{
  private readonly ICacheService _cacheService;
  private readonly IKrakenarSettings _krakenar;
  private readonly IRealmService _realmService;

  public RealmGateway(ICacheService cacheService, IKrakenarSettings krakenar, IRealmService realmService)
  {
    _cacheService = cacheService;
    _krakenar = krakenar;
    _realmService = realmService;
  }

  public async Task<Realm> FindAsync(CancellationToken cancellationToken)
  {
    return await _realmService.ReadAsync(id: null, _krakenar.Realm, cancellationToken) ?? throw new InvalidOperationException($"The realm 'Slug={_krakenar.Realm}' was not found.");
  }

  public async Task<Guid> FindIdCachedAsync(CancellationToken cancellationToken)
  {
    Guid? realmId = _cacheService.RealmId;
    if (!realmId.HasValue)
    {
      Realm realm = await FindAsync(cancellationToken);
      realmId = realm.Id;
      _cacheService.RealmId = realmId.Value;
    }
    return realmId.Value;
  }
}

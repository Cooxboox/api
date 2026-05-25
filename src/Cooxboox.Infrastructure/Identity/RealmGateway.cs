using Cooxboox.Core.Identity;
using Krakenar.Client;
using Krakenar.Contracts.Realms;

namespace Cooxboox.Infrastructure.Identity;

internal class RealmGateway : IRealmGateway
{
  private readonly IRealmService _realmService;
  private readonly KrakenarSettings _settings;

  public RealmGateway(IRealmService realmService, KrakenarSettings settings)
  {
    _realmService = realmService;
    _settings = settings;
  }

  public async Task<Realm> FindAsync(CancellationToken cancellationToken)
  {
    return await _realmService.ReadAsync(id: null, _settings.Realm, cancellationToken) ?? throw new InvalidOperationException($"The realm 'Slug={_settings.Realm}' was not found.");
  }
}

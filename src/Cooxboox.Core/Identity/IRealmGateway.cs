using Krakenar.Contracts.Realms;

namespace Cooxboox.Core.Identity;

public interface IRealmGateway
{
  Task<Realm> FindAsync(CancellationToken cancellationToken = default);
  Task<Guid> FindIdCachedAsync(CancellationToken cancellationToken = default);
}

using Krakenar.Contracts.Realms;

namespace Cooxboox.Core.Identity;

public interface IRealmGateway
{
  Task<Realm> FindAsync(CancellationToken cancellationToken = default);
}

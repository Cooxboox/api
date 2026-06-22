using Krakenar.Contracts.Users;

namespace Cooxboox.Core.Identity;

public interface IUserGateway
{
  Task<IReadOnlyCollection<User>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

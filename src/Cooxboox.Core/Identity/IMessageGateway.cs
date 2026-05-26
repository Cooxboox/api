using Krakenar.Contracts.Passwords;
using Krakenar.Contracts.Users;

namespace Cooxboox.Core.Identity;

public interface IMessageGateway
{
  Task<Guid> SendEmailVerificationAsync(string emailAddress, string locale, string token, CancellationToken cancellationToken = default);
  Task<Guid> SendEmailVerificationAsync(User user, string locale, string token, CancellationToken cancellationToken = default);

  Task<Guid> SendMultiFactorAuthenticationAsync(User user, string? locale, OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);
}

using Krakenar.Contracts.ApiKeys;

namespace Cooxboox.Core.Identity;

public interface IApiKeyGateway
{
  Task<ApiKey> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken = default);
}

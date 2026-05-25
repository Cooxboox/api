using Cooxboox.Core;
using Cooxboox.Core.Identity;
using Krakenar.Client;
using Krakenar.Client.Sessions;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;

namespace Cooxboox.Infrastructure.Identity;

internal class SessionGateway : ISessionGateway
{
  private readonly IContext _context;
  private readonly ISessionClient _sessionClient;

  public SessionGateway(IContext context, ISessionClient sessionClient)
  {
    _context = context;
    _sessionClient = sessionClient;
  }

  public async Task<Session> CreateAsync(User user, CancellationToken cancellationToken)
  {
    CreateSessionPayload payload = new(user.Id.ToString(), isPersistent: true, _context.GetSessionCustomAttributes());
    RequestContext context = new RequestContextBuilder(cancellationToken).WithUser(user).Build();
    return await _sessionClient.CreateAsync(payload, context);
  }

  public async Task<Session> RenewAsync(string refreshToken, CancellationToken cancellationToken)
  {
    RenewSessionPayload payload = new(refreshToken, _context.GetSessionCustomAttributes());
    return await _sessionClient.RenewAsync(payload, cancellationToken);
  }

  public async Task<Session> SignInAsync(User user, string password, CancellationToken cancellationToken)
  {
    SignInSessionPayload payload = new(user.UniqueName, password, isPersistent: true, _context.GetSessionCustomAttributes());
    RequestContext context = new RequestContextBuilder(cancellationToken).WithUser(user).Build();
    return await _sessionClient.SignInAsync(payload, context);
  }
}

using Cooxboox.Constants;
using Cooxboox.Core.Identity;
using Cooxboox.Extensions;
using Krakenar.Contracts.Sessions;

namespace Cooxboox.Middlewares;

internal class RenewSession
{
  private readonly RequestDelegate _next;

  public RenewSession(RequestDelegate next)
  {
    _next = next;
  }

  public virtual async Task InvokeAsync(HttpContext context, ISessionGateway sessionGateway)
  {
    if (!context.IsSignedIn())
    {
      if (context.Request.Cookies.TryGetValue(Cookies.RefreshToken, out string? refreshToken) && refreshToken is not null)
      {
        try
        {
          Session session = await sessionGateway.RenewAsync(refreshToken);
          context.SignIn(session);
        }
        catch (Exception)
        {
          context.Response.Cookies.Delete(Cookies.RefreshToken);
        }
      }
    }

    await _next(context);
  }
}

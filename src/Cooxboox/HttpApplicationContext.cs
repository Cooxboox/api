using Cooxboox.Core;
using Cooxboox.Extensions;
using Cooxboox.Infrastructure;

namespace Cooxboox;

internal class HttpApplicationContext : IContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private HttpContext Context => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("The HttpContext is required.");

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");

  public Guid? TryGetUserId() => Context.GetUser()?.Id;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
  {
    CooxbooxContext cooxboox = Context.RequestServices.GetRequiredService<CooxbooxContext>();
    return await cooxboox.SaveChangesAsync(cancellationToken);
  }
}

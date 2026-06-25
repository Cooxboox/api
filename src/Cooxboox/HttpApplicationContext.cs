using Cooxboox.Core;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Extensions;
using Cooxboox.Infrastructure;
using Krakenar.Contracts.Users;

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

  public Guid KitchenId => TryGetKitchenId() ?? throw new InvalidOperationException("A kitchen is required.");
  public bool IsKitchenOwner
  {
    get
    {
      KitchenModel? kitchen = Context.GetKitchen();
      User? user = Context.GetUser();
      return kitchen is not null && user is not null && kitchen.Owner.Id == user.Id;
    }
  }

  public Guid? TryGetKitchenId() => Context.GetKitchen()?.Id;
  public Guid? TryGetUserId() => Context.GetUser()?.Id;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
  {
    CooxbooxContext cooxboox = Context.RequestServices.GetRequiredService<CooxbooxContext>();
    return await cooxboox.SaveChangesAsync(cancellationToken);
  }
}

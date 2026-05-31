using Cooxboox.Core.Kitchens;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Permissions;

public interface IPermissionService
{
  Task CheckAsync(string action, CancellationToken cancellationToken = default);
  Task CheckAsync(string action, object? resource, CancellationToken cancellationToken = default);
}

internal class PermissionService : IPermissionService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IPermissionService, PermissionService>();
  }

  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;

  public PermissionService(IContext context, IKitchenQuerier kitchenQuerier)
  {
    _context = context;
    _kitchenQuerier = kitchenQuerier;
  }

  public async Task CheckAsync(string action, CancellationToken cancellationToken)
  {
    await CheckAsync(action, resource: null, cancellationToken);
  }
  public async Task CheckAsync(string action, object? resource, CancellationToken cancellationToken)
  {
    bool isAllowed = false;

    Entity? entity = null;
    if (resource is null)
    {
      isAllowed = await IsAllowedAsync(action, cancellationToken);
    }
    else if (resource is Kitchen kitchen)
    {
      entity = new Entity(Kitchen.EntityKind, kitchen.EntityId);
      isAllowed = IsAllowed(action, kitchen);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(action, _context.ActorId, entity);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    if (action == Actions.CreateKitchen)
    {
      return await _kitchenQuerier.CountAsync(cancellationToken) < 1;
    }

    return false;
  }

  private bool IsAllowed(string action, Kitchen kitchen)
  {
    if (action == Actions.Update)
    {
      return kitchen.OwnerId == _context.UserId;
    }

    return false;
  }
}

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
      entity = kitchen.Entity;
      isAllowed = IsAllowed(action, kitchen);
    }
    else if (resource is IEntityProvider provider)
    {
      entity = provider.Entity;
      isAllowed = IsAllowed(action, entity);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(action, _context.ActorId, entity);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    switch (action)
    {
      case Actions.CreateIngredientCategory:
      case Actions.CreateIngredientType:
      case Actions.CreateRecipeType:
        return _context.IsKitchenOwner;
      case Actions.CreateKitchen:
        int count = await _kitchenQuerier.CountAsync(cancellationToken);
        return count < 1;
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

  private bool IsAllowed(string action, Entity entity)
  {
    switch (action)
    {
      case Actions.Publish:
      case Actions.Unpublish:
      case Actions.Update:
        return _context.IsKitchenOwner && _context.KitchenId.Equals(entity.KitchenId);
    }

    return false;
  }
}

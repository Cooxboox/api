using Cooxboox.Core.Kitchens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Permissions;

public interface IPermissionService
{
  Task CheckAsync(string action, CancellationToken cancellationToken = default);
  Task CheckAsync(string action, IResource? resource, CancellationToken cancellationToken = default);
}

internal class PermissionService : IPermissionService
{
  public static void Register(IServiceCollection services)
  {
    services.AddSingleton(serviceProvider => PermissionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddTransient<IPermissionService, PermissionService>();
  }

  private readonly IContext _context;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly PermissionSettings _settings;

  public PermissionService(IContext context, IKitchenRepository kitchenRepository, PermissionSettings settings)
  {
    _context = context;
    _kitchenRepository = kitchenRepository;
    _settings = settings;
  }

  public async Task CheckAsync(string action, CancellationToken cancellationToken)
  {
    await CheckAsync(action, resource: null, cancellationToken);
  }
  public async Task CheckAsync(string action, IResource? resource, CancellationToken cancellationToken)
  {
    bool isAllowed = false;

    ResourceIdentifier? identifier = null;
    if (resource is null)
    {
      isAllowed = await IsAllowedAsync(action, cancellationToken);
    }
    else
    {
      identifier = resource.Identifier;
      isAllowed = resource is Kitchen kitchen ? IsAllowed(action, kitchen) : IsAllowed(action, identifier);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(_context.TryGetUserId(), action, identifier);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    switch (action)
    {
      case Actions.CreateKitchen:
        int count = await _kitchenRepository.CountAsync(cancellationToken);
        return count < _settings.KitchenLimit;
      default:
        return false;
    }
  }

  private bool IsAllowed(string action, Kitchen kitchen)
  {
    switch (action)
    {
      case Actions.Update:
        return kitchen.OwnerId == _context.TryGetUserId();
      default:
        return false;
    }
  }

  private bool IsAllowed(string action, ResourceIdentifier resource)
  {
    switch (action)
    {
      case Actions.Update:
        return resource.KitchenId == _context.TryGetKitchenId() && _context.IsKitchenOwner;
      default:
        return false;
    }
  }
}

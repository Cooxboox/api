using Cooxboox.Core.Kitchens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    services.AddSingleton(serviceProvider => PermissionSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddTransient<IPermissionService, PermissionService>();
  }

  private readonly IContext _context;
  private readonly IDbContext _database;
  private readonly PermissionSettings _settings;

  public PermissionService(IContext context, IDbContext database, PermissionSettings settings)
  {
    _context = context;
    _database = database;
    _settings = settings;
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
      entity = kitchen.GetEntity();
      isAllowed = IsAllowed(action, kitchen);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(_context.TryGetUserId(), action, entity);
    }
  }

  private async Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken)
  {
    switch (action)
    {
      case Actions.CreateKitchen:
        int count = await _database.Kitchens.CountAsync(x => x.OwnerId == _context.UserId, cancellationToken);
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
        return kitchen.OwnerId == _context.UserId;
      default:
        return false;
    }
  }
}

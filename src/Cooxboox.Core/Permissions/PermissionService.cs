using Cooxboox.Core.Contents;
using Krakenar.Contracts.Contents;
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

  public PermissionService(IContext context)
  {
    _context = context;
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
    else if (resource is Content content)
    {
      entity = new Entity(content.ContentType.UniqueName, Guid.Parse(content.Invariant.UniqueName));
      isAllowed = IsAllowed(action, content);
    }

    if (!isAllowed)
    {
      throw new PermissionDeniedException(action, _context.ActorId, entity);
    }
  }

  private Task<bool> IsAllowedAsync(string action, CancellationToken cancellationToken) => Task.FromResult(true); // TODO(fpion): implement

  private bool IsAllowed(string action, Content content)
  {
    if (action == Actions.Update && content.ContentType.UniqueName == "Kitchen")
    {
      string owner = _context.UserId.EntityId.ToString();
      return content.Invariant.FieldValues.Any(field => field.Id == KitchenDefinition.Owner && field.Value == owner);
    }
    return false;
  }
}

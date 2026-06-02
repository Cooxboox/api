using Cooxboox.Core.Kitchens.Commands;
using Cooxboox.Core.Kitchens.Models;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenService
{
  Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
}

internal class KitchenService : IKitchenService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IKitchenService, KitchenService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>, CreateOrReplaceKitchenCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public KitchenService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

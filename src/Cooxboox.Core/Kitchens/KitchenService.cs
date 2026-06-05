using Cooxboox.Core.Kitchens.Commands;
using Cooxboox.Core.Kitchens.Models;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenService
{
  Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id = null, string? language = null, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken = default);
}

internal class KitchenService : IKitchenService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IKitchenService, KitchenService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>, CreateOrReplaceKitchenCommandHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public KitchenService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id, string? language, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenCommand command = new(payload, id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken)
  {
    UpdateKitchenCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

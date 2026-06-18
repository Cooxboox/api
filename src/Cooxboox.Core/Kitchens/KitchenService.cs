using Cooxboox.Core.Kitchens.Commands;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Kitchens.Queries;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenService
{
  Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken = default);
}

internal class KitchenService : IKitchenService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IKitchenService, KitchenService>();
    services.AddTransient<IKitchenManager, KitchenManager>();
    services.AddTransient<ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>, CreateOrReplaceKitchenCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateKitchenCommand, KitchenModel?>, UpdateKitchenCommandHandler>();
    services.AddTransient<IQueryHandler<ReadKitchenQuery, KitchenModel?>, ReadKitchenQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public KitchenService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadKitchenQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken)
  {
    UpdateKitchenCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

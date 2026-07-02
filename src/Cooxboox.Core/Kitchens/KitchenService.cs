using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Kitchens.Queries;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenService
{
  Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}

internal class KitchenService : IKitchenService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IKitchenService, KitchenService>();
    services.AddTransient<IQueryHandler<ReadKitchenQuery, KitchenModel?>, ReadKitchenQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public KitchenService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadKitchenQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }
}

using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class KitchenEvents : IEventHandler<KitchenCreated>, IEventHandler<KitchenDeleted>, IEventHandler<KitchenUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<KitchenCreated>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenDeleted>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenUpdated>, KitchenEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public KitchenEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(KitchenCreated @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null)
    {
      kitchen = new KitchenEntity(@event);

      _cooxboox.Kitchens.Add(kitchen);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenDeleted @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null)
    {
      _cooxboox.Kitchens.Remove(kitchen);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenUpdated @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.Update(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

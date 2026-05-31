using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class KitchenEvents : IEventHandler<KitchenConfidentialityChanged>,
  IEventHandler<KitchenCreated>,
  IEventHandler<KitchenDeleted>,
  IEventHandler<KitchenLocaleChanged>,
  IEventHandler<KitchenLocalePublished>,
  IEventHandler<KitchenLocaleRemoved>,
  IEventHandler<KitchenLocaleUnpublished>,
  IEventHandler<KitchenRenamed>,
  IEventHandler<KitchenSlugChanged>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<KitchenConfidentialityChanged>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenCreated>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenDeleted>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocaleChanged>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocalePublished>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocaleRemoved>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocaleUnpublished>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenRenamed>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenSlugChanged>, KitchenEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public KitchenEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(KitchenConfidentialityChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.SetConfidentiality(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
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

  public async Task HandleAsync(KitchenLocaleChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.Include(x => x.Locales).SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.SetLocale(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocalePublished @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.Include(x => x.Locales).SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.PublishLocale(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocaleRemoved @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.Include(x => x.Locales).SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      KitchenLocaleEntity? locale = kitchen.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.KitchenLocales.Remove(locale);
      }
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocaleUnpublished @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.Include(x => x.Locales).SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.UnpublishLocale(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenRenamed @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.Rename(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenSlugChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is not null && kitchen.Version == (@event.Version - 1))
    {
      kitchen.SetSlug(@event);
      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

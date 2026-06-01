using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class KitchenEvents : IEventHandler<KitchenCreated>,
  IEventHandler<KitchenConfidentialityChanged>,
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
    services.AddTransient<IEventHandler<KitchenCreated>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenConfidentialityChanged>, KitchenEvents>();
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

  public async Task HandleAsync(KitchenCreated @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null)
    {
      kitchen = new KitchenEntity(@event);

      _cooxboox.Kitchens.Add(kitchen);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
    else
    {
      // TODO(fpion): implement
    }
  }

  public async Task HandleAsync(KitchenConfidentialityChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      kitchen.SetConfidentiality(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenDeleted @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null)
    {
      // TODO(fpion): implement
    }
    else
    {
      _cooxboox.Kitchens.Remove(kitchen);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocaleChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      kitchen.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocalePublished @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens
      .Include(x => x.Locales).ThenInclude(x => x.PublishedLocale)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      kitchen.PublishLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenLocaleRemoved @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
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
    KitchenEntity? kitchen = await _cooxboox.Kitchens
      .Include(x => x.Locales).ThenInclude(x => x.PublishedLocale)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      PublishedKitchenLocaleEntity? publishedLocale = kitchen.UnpublishLocale(@event);
      if (publishedLocale is not null)
      {
        _cooxboox.PublishedKitchenLocales.Remove(publishedLocale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenRenamed @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      kitchen.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(KitchenSlugChanged @event, CancellationToken cancellationToken)
  {
    KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (kitchen is null || kitchen.Version != (@event.Version - 1))
    {
      // TODO(fpion): implement
    }
    else
    {
      kitchen.SetSlug(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

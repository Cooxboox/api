using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class KitchenEvents : IEventHandler<KitchenAnnotated>,
  IEventHandler<KitchenCreated>,
  IEventHandler<KitchenDeleted>,
  IEventHandler<KitchenLocaleChanged>,
  IEventHandler<KitchenLocaleRemoved>,
  IEventHandler<KitchenPublished>,
  IEventHandler<KitchenRenamed>,
  IEventHandler<KitchenSlugChanged>,
  IEventHandler<KitchenUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<KitchenAnnotated>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenCreated>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenDeleted>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocaleChanged>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenLocaleRemoved>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenPublished>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenRenamed>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenSlugChanged>, KitchenEvents>();
    services.AddTransient<IEventHandler<KitchenUnpublished>, KitchenEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public KitchenEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(KitchenAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (kitchen is null)
      {
        kitchen = new KitchenEntity(@event);

        _cooxboox.Kitchens.Add(kitchen);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(KitchenDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (kitchen is not null)
      {
        _cooxboox.Kitchens.Remove(kitchen);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(KitchenLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      KitchenLocaleEntity? locale = kitchen.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.KitchenLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenPublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenSlugChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.SetSlug(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(KitchenUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      KitchenEntity? kitchen = await _cooxboox.Kitchens
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, kitchen);

      kitchen.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

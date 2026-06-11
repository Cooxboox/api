using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class IngredientTypeEvents : IEventHandler<IngredientTypeAnnotated>,
  IEventHandler<IngredientTypeCreated>,
  IEventHandler<IngredientTypeDeleted>,
  IEventHandler<IngredientTypeLocaleChanged>,
  IEventHandler<IngredientTypeLocaleRemoved>,
  IEventHandler<IngredientTypePublished>,
  IEventHandler<IngredientTypeRenamed>,
  IEventHandler<IngredientTypeUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<IngredientTypeAnnotated>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeCreated>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeDeleted>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeLocaleChanged>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeLocaleRemoved>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypePublished>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeRenamed>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeUnpublished>, IngredientTypeEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public IngredientTypeEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(IngredientTypeAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      ingredientType.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (ingredientType is null)
      {
        IngredientTypeId ingredientTypeId = new(@event.StreamId);
        KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == ingredientTypeId.KitchenId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={ingredientTypeId.KitchenId}' was not found.");

        ingredientType = new IngredientTypeEntity(kitchen, @event);

        _cooxboox.IngredientTypes.Add(ingredientType);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (ingredientType is not null)
      {
        _cooxboox.IngredientTypes.Remove(ingredientType);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      ingredientType.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      IngredientTypeLocaleEntity? locale = ingredientType.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.IngredientTypeLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypePublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      ingredientType.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      ingredientType.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientTypeUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientType);

      ingredientType.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

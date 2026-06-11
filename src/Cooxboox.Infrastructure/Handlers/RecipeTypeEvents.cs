using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.RecipeTypes.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class RecipeTypeEvents : IEventHandler<RecipeTypeAnnotated>,
  IEventHandler<RecipeTypeCreated>,
  IEventHandler<RecipeTypeDeleted>,
  IEventHandler<RecipeTypeLocaleChanged>,
  IEventHandler<RecipeTypeLocaleRemoved>,
  IEventHandler<RecipeTypePublished>,
  IEventHandler<RecipeTypeRenamed>,
  IEventHandler<RecipeTypeUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<RecipeTypeAnnotated>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeCreated>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeDeleted>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeLocaleChanged>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeLocaleRemoved>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypePublished>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeRenamed>, RecipeTypeEvents>();
    services.AddTransient<IEventHandler<RecipeTypeUnpublished>, RecipeTypeEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public RecipeTypeEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(RecipeTypeAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      recipeType.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipeType is null)
      {
        RecipeTypeId recipeTypeId = new(@event.StreamId);
        KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == recipeTypeId.KitchenId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={recipeTypeId.KitchenId}' was not found.");

        recipeType = new RecipeTypeEntity(kitchen, @event);

        _cooxboox.RecipeTypes.Add(recipeType);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipeType is not null)
      {
        _cooxboox.RecipeTypes.Remove(recipeType);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      recipeType.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      RecipeTypeLocaleEntity? locale = recipeType.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.RecipeTypeLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypePublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      recipeType.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      recipeType.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeTypeUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeTypeEntity? recipeType = await _cooxboox.RecipeTypes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeType);

      recipeType.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

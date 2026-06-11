using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class IngredientCategoryEvents : IEventHandler<IngredientCategoryAnnotated>,
  IEventHandler<IngredientCategoryCreated>,
  IEventHandler<IngredientCategoryDeleted>,
  IEventHandler<IngredientCategoryLocaleChanged>,
  IEventHandler<IngredientCategoryLocaleRemoved>,
  IEventHandler<IngredientCategoryPublished>,
  IEventHandler<IngredientCategoryRenamed>,
  IEventHandler<IngredientCategoryUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<IngredientCategoryAnnotated>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryCreated>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryDeleted>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryLocaleChanged>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryLocaleRemoved>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryPublished>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryRenamed>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryUnpublished>, IngredientCategoryEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public IngredientCategoryEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(IngredientCategoryAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      ingredientCategory.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (ingredientCategory is null)
      {
        IngredientCategoryId ingredientCategoryId = new(@event.StreamId);
        KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == ingredientCategoryId.KitchenId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={ingredientCategoryId.KitchenId}' was not found.");

        ingredientCategory = new IngredientCategoryEntity(kitchen, @event);

        _cooxboox.IngredientCategories.Add(ingredientCategory);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (ingredientCategory is not null)
      {
        _cooxboox.IngredientCategories.Remove(ingredientCategory);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      ingredientCategory.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      IngredientCategoryLocaleEntity? locale = ingredientCategory.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.IngredientCategoryLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryPublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      ingredientCategory.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      ingredientCategory.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(IngredientCategoryUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, ingredientCategory);

      ingredientCategory.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class RecipeEvents : IEventHandler<RecipeAnnotated>,
  IEventHandler<RecipeCreated>,
  IEventHandler<RecipeDeleted>,
  IEventHandler<RecipeLocaleChanged>,
  IEventHandler<RecipeLocaleRemoved>,
  IEventHandler<RecipePublished>,
  IEventHandler<RecipeRenamed>,
  IEventHandler<RecipeUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<RecipeAnnotated>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeCreated>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeDeleted>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeLocaleChanged>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeLocaleRemoved>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipePublished>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeRenamed>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeUnpublished>, RecipeEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public RecipeEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(RecipeAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      recipe.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipe is null)
      {
        RecipeId recipeId = new(@event.StreamId);
        KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == recipeId.KitchenId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={recipeId.KitchenId}' was not found.");

        recipe = new RecipeEntity(kitchen, @event);

        _cooxboox.Recipes.Add(recipe);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipe is not null)
      {
        _cooxboox.Recipes.Remove(recipe);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      recipe.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      RecipeLocaleEntity? locale = recipe.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.RecipeLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipePublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      recipe.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      recipe.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeEntity? recipe = await _cooxboox.Recipes
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipe);

      recipe.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

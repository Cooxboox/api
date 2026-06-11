using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Events;
using Cooxboox.Infrastructure.Entities;
using Cooxboox.Infrastructure.Outbox;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class RecipeCategoryEvents : IEventHandler<RecipeCategoryAnnotated>,
  IEventHandler<RecipeCategoryCreated>,
  IEventHandler<RecipeCategoryDeleted>,
  IEventHandler<RecipeCategoryLocaleChanged>,
  IEventHandler<RecipeCategoryLocaleRemoved>,
  IEventHandler<RecipeCategoryPublished>,
  IEventHandler<RecipeCategoryRenamed>,
  IEventHandler<RecipeCategoryUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<RecipeCategoryAnnotated>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryCreated>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryDeleted>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryLocaleChanged>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryLocaleRemoved>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryPublished>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryRenamed>, RecipeCategoryEvents>();
    services.AddTransient<IEventHandler<RecipeCategoryUnpublished>, RecipeCategoryEvents>();
  }

  private readonly CooxbooxContext _cooxboox;
  private readonly IOutboxService _outbox;

  public RecipeCategoryEvents(CooxbooxContext cooxboox, IOutboxService outbox)
  {
    _cooxboox = cooxboox;
    _outbox = outbox;
  }

  public async Task HandleAsync(RecipeCategoryAnnotated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      recipeCategory.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryCreated @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipeCategory is null)
      {
        RecipeCategoryId recipeCategoryId = new(@event.StreamId);
        KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == recipeCategoryId.KitchenId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={recipeCategoryId.KitchenId}' was not found.");

        recipeCategory = new RecipeCategoryEntity(kitchen, @event);

        _cooxboox.RecipeCategories.Add(recipeCategory);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryDeleted @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      if (recipeCategory is not null)
      {
        _cooxboox.RecipeCategories.Remove(recipeCategory);

        await _cooxboox.SaveChangesAsync(cancellationToken);
      }
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryLocaleChanged @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      recipeCategory.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryLocaleRemoved @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      RecipeCategoryLocaleEntity? locale = recipeCategory.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.RecipeCategoryLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryPublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      recipeCategory.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryRenamed @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      recipeCategory.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);

  public async Task HandleAsync(RecipeCategoryUnpublished @event, CancellationToken cancellationToken) => await _outbox.HandleAsync(
    @event,
    async (@event, cancellationToken) =>
    {
      RecipeCategoryEntity? recipeCategory = await _cooxboox.RecipeCategories
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
      UnexpectedVersionException.ThrowIfUnexpected(@event, recipeCategory);

      recipeCategory.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    },
    cancellationToken);
}

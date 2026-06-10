using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class RecipeEvents : IEventHandler<RecipeCreated>,
  IEventHandler<RecipeDeleted>,
  IEventHandler<RecipeLocaleChanged>,
  IEventHandler<RecipeLocaleRemoved>,
  IEventHandler<RecipePublished>,
  IEventHandler<RecipeUnpublished>,
  IEventHandler<RecipeUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<RecipeCreated>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeDeleted>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeLocaleChanged>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeLocaleRemoved>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipePublished>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeUnpublished>, RecipeEvents>();
    services.AddTransient<IEventHandler<RecipeUpdated>, RecipeEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public RecipeEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(RecipeCreated @event, CancellationToken cancellationToken)
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
  }

  public async Task HandleAsync(RecipeDeleted @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null)
    {
      _cooxboox.Recipes.Remove(recipe);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(RecipeLocaleChanged @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null && recipe.Version == (@event.Version - 1))
    {
      recipe.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(RecipeLocaleRemoved @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null && recipe.Version == (@event.Version - 1))
    {
      RecipeLocaleEntity? locale = recipe.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.RecipeLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(RecipePublished @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null && recipe.Version == (@event.Version - 1))
    {
      recipe.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(RecipeUnpublished @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null && recipe.Version == (@event.Version - 1))
    {
      recipe.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(RecipeUpdated @event, CancellationToken cancellationToken)
  {
    RecipeEntity? recipe = await _cooxboox.Recipes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (recipe is not null && recipe.Version == (@event.Version - 1))
    {
      recipe.Update(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

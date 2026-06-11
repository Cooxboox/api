using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class IngredientEvents : IEventHandler<IngredientAnnotated>,
  IEventHandler<IngredientCreated>,
  IEventHandler<IngredientDeleted>,
  IEventHandler<IngredientLocaleChanged>,
  IEventHandler<IngredientLocaleRemoved>,
  IEventHandler<IngredientPublished>,
  IEventHandler<IngredientRenamed>,
  IEventHandler<IngredientUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<IngredientAnnotated>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientCreated>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientDeleted>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientLocaleChanged>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientLocaleRemoved>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientPublished>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientRenamed>, IngredientEvents>();
    services.AddTransient<IEventHandler<IngredientUnpublished>, IngredientEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public IngredientEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(IngredientAnnotated @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      ingredient.Annotate(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCreated @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients.AsNoTracking().SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is null)
    {
      IngredientId ingredientId = new(@event.StreamId);
      KitchenEntity kitchen = await _cooxboox.Kitchens.SingleOrDefaultAsync(x => x.StreamId == ingredientId.KitchenId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The kitchen entity 'StreamId={ingredientId.KitchenId}' was not found.");

      ingredient = new IngredientEntity(kitchen, @event);

      _cooxboox.Ingredients.Add(ingredient);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientDeleted @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null)
    {
      _cooxboox.Ingredients.Remove(ingredient);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientLocaleChanged @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      ingredient.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientLocaleRemoved @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      IngredientLocaleEntity? locale = ingredient.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.IngredientLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientPublished @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      ingredient.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientRenamed @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      ingredient.Rename(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientUnpublished @event, CancellationToken cancellationToken)
  {
    IngredientEntity? ingredient = await _cooxboox.Ingredients
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredient is not null && ingredient.Version == (@event.Version - 1))
    {
      ingredient.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class IngredientCategoryEvents : IEventHandler<IngredientCategoryCreated>,
  IEventHandler<IngredientCategoryDeleted>,
  IEventHandler<IngredientCategoryLocaleChanged>,
  IEventHandler<IngredientCategoryLocaleRemoved>,
  IEventHandler<IngredientCategoryPublished>,
  IEventHandler<IngredientCategoryUnpublished>,
  IEventHandler<IngredientCategoryUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<IngredientCategoryCreated>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryDeleted>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryLocaleChanged>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryLocaleRemoved>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryPublished>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryUnpublished>, IngredientCategoryEvents>();
    services.AddTransient<IEventHandler<IngredientCategoryUpdated>, IngredientCategoryEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public IngredientCategoryEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(IngredientCategoryCreated @event, CancellationToken cancellationToken)
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
  }

  public async Task HandleAsync(IngredientCategoryDeleted @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null)
    {
      _cooxboox.IngredientCategories.Remove(ingredientCategory);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCategoryLocaleChanged @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null && ingredientCategory.Version == (@event.Version - 1))
    {
      ingredientCategory.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCategoryLocaleRemoved @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null && ingredientCategory.Version == (@event.Version - 1))
    {
      IngredientCategoryLocaleEntity? locale = ingredientCategory.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.IngredientCategoryLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCategoryPublished @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null && ingredientCategory.Version == (@event.Version - 1))
    {
      ingredientCategory.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCategoryUnpublished @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null && ingredientCategory.Version == (@event.Version - 1))
    {
      ingredientCategory.Unpublish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientCategoryUpdated @event, CancellationToken cancellationToken)
  {
    IngredientCategoryEntity? ingredientCategory = await _cooxboox.IngredientCategories.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientCategory is not null && ingredientCategory.Version == (@event.Version - 1))
    {
      ingredientCategory.Update(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

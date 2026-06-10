using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Events;
using Cooxboox.Infrastructure.Entities;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Handlers;

internal class IngredientTypeEvents : IEventHandler<IngredientTypeCreated>,
  IEventHandler<IngredientTypeDeleted>,
  IEventHandler<IngredientTypeLocaleChanged>,
  IEventHandler<IngredientTypeLocaleRemoved>,
  IEventHandler<IngredientTypePublished>,
  IEventHandler<IngredientTypeUpdated>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<IngredientTypeCreated>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeDeleted>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeLocaleChanged>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeLocaleRemoved>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypePublished>, IngredientTypeEvents>();
    services.AddTransient<IEventHandler<IngredientTypeUpdated>, IngredientTypeEvents>();
  }

  private readonly CooxbooxContext _cooxboox;

  public IngredientTypeEvents(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task HandleAsync(IngredientTypeCreated @event, CancellationToken cancellationToken)
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
  }

  public async Task HandleAsync(IngredientTypeDeleted @event, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientType is not null)
    {
      _cooxboox.IngredientTypes.Remove(ingredientType);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientTypeLocaleChanged @event, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientType is not null && ingredientType.Version == (@event.Version - 1))
    {
      ingredientType.SetLocale(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientTypeLocaleRemoved @event, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientType is not null && ingredientType.Version == (@event.Version - 1))
    {
      IngredientTypeLocaleEntity? locale = ingredientType.RemoveLocale(@event);
      if (locale is not null)
      {
        _cooxboox.IngredientTypeLocales.Remove(locale);
      }

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientTypePublished @event, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes
      .Include(x => x.Locales)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientType is not null && ingredientType.Version == (@event.Version - 1))
    {
      ingredientType.Publish(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task HandleAsync(IngredientTypeUpdated @event, CancellationToken cancellationToken)
  {
    IngredientTypeEntity? ingredientType = await _cooxboox.IngredientTypes.SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (ingredientType is not null && ingredientType.Version == (@event.Version - 1))
    {
      ingredientType.Update(@event);

      await _cooxboox.SaveChangesAsync(cancellationToken);
    }
  }
}

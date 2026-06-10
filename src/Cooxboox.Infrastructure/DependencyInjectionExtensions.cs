using Cooxboox.Core.Actors;
using Cooxboox.Core.Caching;
using Cooxboox.Core.Identity;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeTypes;
using Cooxboox.Infrastructure.Actors;
using Cooxboox.Infrastructure.Caching;
using Cooxboox.Infrastructure.Handlers;
using Cooxboox.Infrastructure.Identity;
using Cooxboox.Infrastructure.Queriers;
using Cooxboox.Infrastructure.Repositories;
using Cooxboox.Infrastructure.Settings;
using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxInfrastructure(this IServiceCollection services)
  {
    return services
      .AddEventHandlers()
      .AddIdentityGateways()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddMemoryCache()
      .AddQueriers()
      .AddRepositories()
      .AddSingleton(serviceProvider => CachingSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => ClientAppSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton(serviceProvider => TokensSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer, EventSerializer>()
      .AddScoped<IActorService, ActorService>()
      .AddScoped<IEventBus, EventBus>()
      .AddTransient<ICommandHandler<MigrateDatabaseCommand, Unit>, MigrateDatabaseCommandHandler>();
  }

  private static IServiceCollection AddEventHandlers(this IServiceCollection services)
  {
    IngredientCategoryEvents.Register(services);
    IngredientTypeEvents.Register(services);
    KitchenEvents.Register(services);
    RecipeCategoryEvents.Register(services);
    RecipeTypeEvents.Register(services);
    return services;
  }

  private static IServiceCollection AddIdentityGateways(this IServiceCollection services)
  {
    return services
      .AddSingleton<IApiKeyGateway, ApiKeyGateway>()
      .AddSingleton<IMessageGateway, MessageGateway>()
      .AddSingleton<IOneTimePasswordGateway, OneTimePasswordGateway>()
      .AddSingleton<IRealmGateway, RealmGateway>()
      .AddSingleton<ISessionGateway, SessionGateway>()
      .AddSingleton<ITokenGateway, TokenGateway>()
      .AddSingleton<IUserGateway, UserGateway>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddScoped<IIngredientCategoryQuerier, IngredientCategoryQuerier>()
      .AddScoped<IIngredientTypeQuerier, IngredientTypeQuerier>()
      .AddScoped<IKitchenQuerier, KitchenQuerier>()
      .AddScoped<IRecipeCategoryQuerier, RecipeCategoryQuerier>()
      .AddScoped<IRecipeTypeQuerier, RecipeTypeQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services
      .AddScoped<IIngredientCategoryRepository, IngredientCategoryRepository>()
      .AddScoped<IIngredientTypeRepository, IngredientTypeRepository>()
      .AddScoped<IKitchenRepository, KitchenRepository>()
      .AddScoped<IRecipeCategoryRepository, RecipeCategoryRepository>()
      .AddScoped<IRecipeTypeRepository, RecipeTypeRepository>();
  }
}

using Cooxboox.Core.Actors;
using Cooxboox.Core.Caching;
using Cooxboox.Core.Contents;
using Cooxboox.Core.Identity;
using Cooxboox.Infrastructure.Actors;
using Cooxboox.Infrastructure.Caching;
using Cooxboox.Infrastructure.Contents;
using Cooxboox.Infrastructure.Identity;
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
      .AddContentGateways()
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

  private static IServiceCollection AddContentGateways(this IServiceCollection services)
  {
    return services.AddSingleton<IContentGateway, ContentGateway>();
  }

  private static IServiceCollection AddEventHandlers(this IServiceCollection services)
  {
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
    return services;
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services;
  }
}

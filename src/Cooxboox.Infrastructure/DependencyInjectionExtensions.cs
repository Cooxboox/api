using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens;
using Cooxboox.Infrastructure.Actors;
using Cooxboox.Infrastructure.Caching;
using Cooxboox.Infrastructure.Identity;
using Cooxboox.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxInfrastructure(this IServiceCollection services)
  {
    ActorService.Register(services);
    CacheService.Register(services);
    return services
      .AddIdentityGateways()
      .AddRepositories();
  }

  private static IServiceCollection AddIdentityGateways(this IServiceCollection services)
  {
    return services.AddSingleton<IUserGateway, UserGateway>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddScoped<IKitchenRepository, KitchenRepository>();
  }
}

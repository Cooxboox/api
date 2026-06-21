using Cooxboox.Core.Kitchens;
using Cooxboox.Infrastructure.Actors;
using Cooxboox.Infrastructure.Caching;
using Cooxboox.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxInfrastructure(this IServiceCollection services)
  {
    ActorService.Register(services);
    CacheService.Register(services);
    return services.AddScoped<IKitchenRepository, KitchenRepository>();
  }
}

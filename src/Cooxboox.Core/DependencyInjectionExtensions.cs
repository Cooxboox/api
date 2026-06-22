using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxCore(this IServiceCollection services)
  {
    KitchenService.Register(services);
    PermissionService.Register(services);

    return services
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>();
  }
}

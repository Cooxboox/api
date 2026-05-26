using Cooxboox.Core.Identity;
using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxCore(this IServiceCollection services)
  {
    return services
      .AddCoreServices()
      .AddLogitarEventSourcing()
      .AddSingleton(serviceProvider => RetrySettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()))
      .AddTransient<ICommandBus, CommandBus>()
      .AddTransient<IQueryBus, QueryBus>();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    IdentityService.Register(services);
    return services;
  }
}

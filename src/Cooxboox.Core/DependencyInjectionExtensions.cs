using Cooxboox.Core.Identity;
using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxCore(this IServiceCollection services)
  {
    return services
      .AddCoreServices()
      .AddLogitarCQRS()
      .AddLogitarEventSourcing();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    IdentityService.Register(services);
    return services;
  }
}

using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational();
  }
}

using Cooxboox.Infrastructure;
using Logitar;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxPostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Cooxboox") ?? configuration.GetConnectionString("PostgreSQL");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new InvalidOperationException("The PostgreSQL connection string was not found.");
    }

    return services
      .AddDbContext<CooxbooxContext>(options => options.UseNpgsql(connectionString, options => options.MigrationsAssembly("Cooxboox.PostgreSQL")))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}

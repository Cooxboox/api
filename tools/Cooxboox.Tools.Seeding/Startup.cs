using Cooxboox.Tools.Seeding.Krakenar.Tasks;
using Krakenar.Client;
using Logitar.CQRS;

namespace Cooxboox.Tools.Seeding;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<SeedingWorker>();
    services.AddKrakenarClient(_configuration);
    services.AddLogitarCQRS();

    services.AddTransient<ICommandHandler<SeedDictionariesTask, Unit>, SeedDictionariesTaskHandler>();
    services.AddTransient<ICommandHandler<SeedLanguagesTask, Unit>, SeedLanguagesTaskHandler>();
    services.AddTransient<ICommandHandler<SeedRealmTask, Unit>, SeedRealmTaskHandler>();
    services.AddTransient<ICommandHandler<SeedSendersTask, Unit>, SeedSendersTaskHandler>();
    services.AddTransient<ICommandHandler<SeedTemplatesTask, Unit>, SeedTemplatesTaskHandler>();
  }
}

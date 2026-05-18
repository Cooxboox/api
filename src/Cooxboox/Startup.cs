using Cooxboox.Extensions;
using Cooxboox.Settings;

namespace Cooxboox;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly ApiSettings _apiSettings;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _apiSettings = ApiSettings.Initialize(configuration);
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers();

    services.AddSingleton(_apiSettings);
    if (_apiSettings.EnableSwagger)
    {
      services.AddCooxbooxSwagger(_apiSettings);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      Configure(application);
    }
  }
  public void Configure(WebApplication application)
  {
    if (_apiSettings.EnableSwagger)
    {
      application.UseCooxbooxSwagger(_apiSettings);
    }

    application.UseHttpsRedirection();
    application.UseAuthorization();

    application.MapControllers();
  }
}

using Cooxboox.Authentication;
using Cooxboox.Core;
using Cooxboox.Extensions;
using Cooxboox.Infrastructure;
using Cooxboox.Middlewares;
using Cooxboox.PostgreSQL;
using Cooxboox.Settings;
using Krakenar.Client;
using Krakenar.Contracts.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Cooxboox;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly ApiSettings _apiSettings;
  private readonly CorsSettings _corsSettings;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _apiSettings = ApiSettings.Initialize(configuration);
    _corsSettings = CorsSettings.Initialize(configuration);
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddCooxbooxCore();
    services.AddCooxbooxInfrastructure();
    services.AddCooxbooxPostgreSQL(_configuration);
    services.AddKrakenarClient(_configuration);
    services.AddSingleton<IContext, HttpApplicationContext>();

    services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    services.AddHttpContextAccessor();

    services.AddSingleton(_corsSettings);
    services.AddCors();

    AuthenticationSettings authenticationSettings = AuthenticationSettings.Initialize(_configuration);
    services.AddSingleton(authenticationSettings);
    string[] authenticationSchemes = GetAuthenticationSchemes(authenticationSettings);
    AuthenticationBuilder authenticationBuilder = services.AddAuthentication()
      .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
      .AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>(Schemes.Bearer, options => { })
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });
    if (authenticationSettings.EnableBasic)
    {
      authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { });
    }
    services.AddSingleton<IOpenAuthenticationService, OpenAuthenticationService>();

    services.AddAuthorizationBuilder().SetDefaultPolicy(new AuthorizationPolicyBuilder(authenticationSchemes).RequireAuthenticatedUser().Build());

    CookiesSettings cookiesSettings = CookiesSettings.Initialize(_configuration);
    services.AddSingleton(cookiesSettings);
    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    services.AddDistributedMemoryCache();

    ErrorSettings settings = ErrorSettings.Initialize(_configuration);
    services.AddSingleton(settings);
    services.AddExceptionHandler<ExceptionHandler>();
    services.AddProblemDetails();

    services.AddHealthChecks().AddDbContextCheck<CooxbooxContext>();

    services.AddSingleton(_apiSettings);
    if (_apiSettings.EnableSwagger)
    {
      services.AddCooxbooxSwagger(_apiSettings);
    }
  }
  private static string[] GetAuthenticationSchemes(AuthenticationSettings settings)
  {
    List<string> schemes = new(capacity: 4)
    {
      Schemes.ApiKey,
      Schemes.Bearer,
      Schemes.Session
    };

    if (settings.EnableBasic)
    {
      schemes.Add(Schemes.Basic);
    }

    return [.. schemes];
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
    application.UseCors(_corsSettings);
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<RenewSession>();
    application.UseAuthentication();
    application.UseAuthorization();
    // TODO(fpion): application.UseMiddleware<ResolveWorld>();

    application.MapControllers();
  }
}

using Cooxboox.Core.Identity;
using Cooxboox.Infrastructure.Identity;
using Cooxboox.Infrastructure.Settings;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.EventSourcing.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddCooxbooxInfrastructure(this IServiceCollection services)
  {
    return services
      .AddIdentityGateways()
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddSingleton(serviceProvider => TokensSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
  }

  private static IServiceCollection AddIdentityGateways(this IServiceCollection services)
  {
    return services
      .AddSingleton<IApiKeyGateway, ApiKeyGateway>()
      .AddSingleton<IMessageGateway, MessageGateway>()
      .AddSingleton<IOneTimePasswordGateway, OneTimePasswordGateway>()
      .AddSingleton<IRealmGateway, RealmGateway>()
      .AddSingleton<ISessionGateway, SessionGateway>()
      .AddSingleton<ITokenGateway, TokenGateway>()
      .AddSingleton<IUserGateway, UserGateway>();
  }
}

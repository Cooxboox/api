using Cooxboox.Settings;
using Krakenar.Contracts.Constants;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using KrakenarHeaders = Krakenar.Contracts.Constants.Headers;

namespace Cooxboox.Extensions;

internal static class SwaggerExtensions
{
  public static IServiceCollection AddCooxbooxSwagger(this IServiceCollection services, ApiSettings settings)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.AddSecurity();
      config.OperationFilterDescriptors.Add(new FilterDescriptor
      {
        Arguments = [],
        Type = typeof(AddHeaderParameters)
      });
      config.SwaggerDoc(name: $"v{settings.Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Francis Pion",
          Url = new Uri("https://www.francispion.ca/", UriKind.Absolute)
        },
        Description = "A multi-tenant cooking backend managing kitchen-scoped ingredients, recipes, media, search, notes, and comments for recipe planning.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Cooxboox/api/blob/main/LICENSE", UriKind.Absolute)
        },
        Title = settings.Title,
        Version = $"v{settings.Version}"
      });
    });
    services.AddTransient<IProblemDetailsWriter, PlainTextProblemDetailsWriter>();

    return services;
  }

  public static void UseCooxbooxSwagger(this IApplicationBuilder builder, ApiSettings settings)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{settings.Version.Major}/swagger.json",
      name: $"{settings.Title} v{settings.Version}"
    ));
  }

  private static void AddSecurity(this SwaggerGenOptions options)
  {
    options.AddSecurityDefinition(Schemes.ApiKey, new OpenApiSecurityScheme
    {
      Description = "Enter your API key in the input below:",
      In = ParameterLocation.Header,
      Name = KrakenarHeaders.ApiKey,
      Scheme = Schemes.ApiKey,
      Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityDefinition(Schemes.Basic, new OpenApiSecurityScheme
    {
      Description = "Enter your credentials in the inputs below:",
      In = ParameterLocation.Header,
      Name = KrakenarHeaders.Authorization,
      Scheme = Schemes.Basic,
      Type = SecuritySchemeType.Http
    });
    options.AddSecurityDefinition(Schemes.Bearer, new OpenApiSecurityScheme
    {
      Description = "Enter your access token in the input below:",
      In = ParameterLocation.Header,
      Name = KrakenarHeaders.Authorization,
      Scheme = Schemes.Bearer,
      Type = SecuritySchemeType.Http
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
      [new OpenApiSecuritySchemeReference(Schemes.ApiKey, document)] = [],
      [new OpenApiSecuritySchemeReference(Schemes.Basic, document)] = [],
      [new OpenApiSecuritySchemeReference(Schemes.Bearer, document)] = []
    });
  }
}

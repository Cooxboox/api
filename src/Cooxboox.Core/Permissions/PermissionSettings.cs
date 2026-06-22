using Logitar;
using Microsoft.Extensions.Configuration;

namespace Cooxboox.Core.Permissions;

internal record PermissionSettings
{
  private const string SectionKey = "Permissions";

  public int KitchenLimit { get; set; }

  public static PermissionSettings Initialize(IConfiguration configuration)
  {
    PermissionSettings settings = configuration.GetSection(SectionKey).Get<PermissionSettings>() ?? new();

    settings.KitchenLimit = EnvironmentHelper.GetInt32("PERMISSIONS_KITCHEN_LIMIT", settings.KitchenLimit);

    return settings;
  }
}

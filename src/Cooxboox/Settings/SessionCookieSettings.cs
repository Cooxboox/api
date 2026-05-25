namespace Cooxboox.Settings;

public record SessionCookieSettings
{
  public SameSiteMode SameSite { get; set; }
}

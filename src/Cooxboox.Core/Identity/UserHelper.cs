namespace Cooxboox.Core.Identity;

public static class UserHelper
{
  public static string NormalizeGender(string gender) => gender.Trim().ToLowerInvariant();
}

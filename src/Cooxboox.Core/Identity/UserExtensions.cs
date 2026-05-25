using Cooxboox.Core.Identity.Models;
using Krakenar.Contracts;
using Krakenar.Contracts.Users;

namespace Cooxboox.Core.Identity;

public static class UserExtensions
{
  private const string MultiFactorAuthenticationModeKey = "MultiFactorAuthenticationMode";
  private const string ProfileCompletedOnKey = "ProfileCompletedOn";

  public static MultiFactorAuthenticationMode GetMultiFactorAuthenticationMode(this User user)
  {
    CustomAttribute? customAttribute = user.GetCustomAttribute(MultiFactorAuthenticationModeKey);
    if (customAttribute is null)
    {
      return MultiFactorAuthenticationMode.None;
    }
    else if (Enum.TryParse(customAttribute.Value, out MultiFactorAuthenticationMode mode) && Enum.IsDefined(mode))
    {
      return mode;
    }
    throw new ArgumentException($"The user 'Id={user.Id}' Multi-Factor Authentication mode '{customAttribute.Value}' is not valid.", nameof(user));
  }

  public static string GetSubject(this User user) => user.Id.ToString();

  public static bool IsProfileCompleted(this User user) => user.GetCustomAttribute(ProfileCompletedOnKey) is not null;

  private static CustomAttribute? GetCustomAttribute(this User user, string key)
  {
    CustomAttribute[] customAttributes = user.CustomAttributes.Where(x => x.Key == key).ToArray();
    if (customAttributes.Length < 1)
    {
      return null;
    }
    else if (customAttributes.Length > 1)
    {
      throw new ArgumentException($"The user 'Id={user.Id}' has multiple ({customAttributes.Length}) custom attributes '{key}'.", nameof(user));
    }
    return customAttributes.Single();
  }
}

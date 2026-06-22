namespace Cooxboox.Core;

internal static class SlugHelper
{
  public static string? Format(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
  public static bool IsValid(string? value) => value is not null && value.Split('_').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
}

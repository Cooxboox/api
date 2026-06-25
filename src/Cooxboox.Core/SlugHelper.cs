namespace Cooxboox.Core;

public static class SlugHelper
{
  public static string? Format(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
  public static bool IsValid(string? value) => value is null
    || (!string.IsNullOrWhiteSpace(value) && value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit)));
}

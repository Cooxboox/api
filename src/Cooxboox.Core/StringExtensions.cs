namespace Cooxboox.Core;

public static class StringExtensions
{
  public static bool IsKebabCase(this string value) => value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
}

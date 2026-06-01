namespace Cooxboox.Core.Localization;

public static class Languages
{
  public const string English = "en";
  public const string French = "fr";

  public static readonly IReadOnlyCollection<string> All = new string[] { English, French }.ToList().AsReadOnly();
  public const string Default = French;
}

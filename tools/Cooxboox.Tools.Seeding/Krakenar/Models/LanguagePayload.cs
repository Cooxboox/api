using Krakenar.Contracts.Localization;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record LanguagePayload : CreateOrReplaceLanguagePayload
{
  public bool IsDefault { get; set; }
}

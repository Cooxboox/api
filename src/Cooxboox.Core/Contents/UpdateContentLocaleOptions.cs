using Cooxboox.Core.Localization;

namespace Cooxboox.Core.Contents;

public record UpdateContentLocaleOptions
{
  public Language? Language { get; set; }

  public string? UniqueName { get; set; }
  public Optional<string>? DisplayName { get; set; }
  public Optional<string>? Description { get; set; }

  public Dictionary<Guid, string> FieldValues { get; set; } = [];
}

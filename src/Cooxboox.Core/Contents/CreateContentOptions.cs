using Cooxboox.Core.Localization;

namespace Cooxboox.Core.Contents;

public record CreateContentOptions
{
  public Guid? Id { get; set; }
  public Language? Language { get; set; }

  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Dictionary<Guid, string> FieldValues { get; set; } = [];
}

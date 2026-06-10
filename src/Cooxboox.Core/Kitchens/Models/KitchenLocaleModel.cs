using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Localization;

namespace Cooxboox.Core.Kitchens.Models;

public record KitchenLocaleModel
{
  public Locale Language { get; set; } = new();

  public string? MetaDescription { get; set; }
  public string? HtmlContent { get; set; }
  public string? Notes { get; set; }

  public long Version { get; set; }
  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public ContentStatus Status { get; set; }
  public long? PublishedVersion { get; set; }
  public Actor? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }
}

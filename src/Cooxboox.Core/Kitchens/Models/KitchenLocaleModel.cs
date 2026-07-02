using Krakenar.Contracts.Actors;

namespace Cooxboox.Core.Kitchens.Models;

public record KitchenLocaleModel : ILocaleModel, IPublishableModel
{
  public string Language { get; set; } = string.Empty;

  public string? MetaDescription { get; set; }
  public string? HtmlContent { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public ContentStatus Status { get; set; }
  public Actor? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }
}

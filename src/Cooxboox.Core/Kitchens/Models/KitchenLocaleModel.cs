using Krakenar.Contracts.Localization;

namespace Cooxboox.Core.Kitchens.Models;

public record KitchenLocaleModel
{
  public Locale Language { get; set; } = new();

  public string? MetaDescription { get; set; }
  public string? HtmlContent { get; set; }
}

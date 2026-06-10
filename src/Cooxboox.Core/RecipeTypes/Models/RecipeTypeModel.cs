using Krakenar.Contracts;
using Krakenar.Contracts.Actors;

namespace Cooxboox.Core.RecipeTypes.Models;

public class RecipeTypeModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Notes { get; set; }

  public ContentStatus Status { get; set; }
  public long? PublishedVersion { get; set; }
  public Actor? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }

  public List<RecipeTypeLocaleModel> Locales { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}

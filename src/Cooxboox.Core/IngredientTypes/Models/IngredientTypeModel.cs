using Krakenar.Contracts;

namespace Cooxboox.Core.IngredientTypes.Models;

public class IngredientTypeModel : Aggregate
{
  public string Name { get; set; } = string.Empty;
  public string? Notes { get; set; }

  public List<IngredientTypeLocaleModel> Locales { get; set; } = [];

  public override string ToString() => $"{Name} | {base.ToString()}";
}

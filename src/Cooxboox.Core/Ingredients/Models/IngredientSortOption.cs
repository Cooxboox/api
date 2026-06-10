using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Ingredients.Models;

public record IngredientSortOption : SortOption
{
  public new IngredientSort Field
  {
    get => Enum.Parse<IngredientSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public IngredientSortOption(IngredientSort field = IngredientSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

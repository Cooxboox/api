using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientTypes.Models;

public record IngredientTypeSortOption : SortOption
{
  public new IngredientTypeSort Field
  {
    get => Enum.Parse<IngredientTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public IngredientTypeSortOption(IngredientTypeSort field = IngredientTypeSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

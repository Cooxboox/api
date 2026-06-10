using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientCategories.Models;

public record IngredientCategorySortOption : SortOption
{
  public new IngredientCategorySort Field
  {
    get => Enum.Parse<IngredientCategorySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public IngredientCategorySortOption(IngredientCategorySort field = IngredientCategorySort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

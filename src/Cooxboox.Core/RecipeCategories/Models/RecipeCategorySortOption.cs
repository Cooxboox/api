using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeCategories.Models;

public record RecipeCategorySortOption : SortOption
{
  public new RecipeCategorySort Field
  {
    get => Enum.Parse<RecipeCategorySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RecipeCategorySortOption(RecipeCategorySort field = RecipeCategorySort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

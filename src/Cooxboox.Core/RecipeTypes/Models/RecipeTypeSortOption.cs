using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeTypes.Models;

public record RecipeTypeSortOption : SortOption
{
  public new RecipeTypeSort Field
  {
    get => Enum.Parse<RecipeTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RecipeTypeSortOption(RecipeTypeSort field = RecipeTypeSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

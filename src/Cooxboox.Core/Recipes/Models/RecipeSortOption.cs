using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Recipes.Models;

public record RecipeSortOption : SortOption
{
  public new RecipeSort Field
  {
    get => Enum.Parse<RecipeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RecipeSortOption(RecipeSort field = RecipeSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

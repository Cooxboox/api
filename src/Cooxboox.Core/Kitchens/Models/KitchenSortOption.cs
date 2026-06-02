using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Kitchens.Models;

public record KitchenSortOption : SortOption
{
  public new KitchenSort Field
  {
    get => Enum.Parse<KitchenSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public KitchenSortOption(KitchenSort field = KitchenSort.CreatedOn, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}

using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class IngredientCategories
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.IngredientCategories), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientCategoryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientCategoryEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(IngredientCategoryEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientCategoryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientCategoryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientCategoryEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(IngredientCategoryEntity.EntityId), Table);
  public static readonly ColumnId IngredientCategoryId = new(nameof(IngredientCategoryEntity.IngredientCategoryId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientCategoryEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(IngredientCategoryEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientCategoryEntity.Notes), Table);
}

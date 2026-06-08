using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class IngredientTypes
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.IngredientTypes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientTypeEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientTypeEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(IngredientTypeEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientTypeEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientTypeEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientTypeEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(IngredientTypeEntity.EntityId), Table);
  public static readonly ColumnId IngredientTypeId = new(nameof(IngredientTypeEntity.IngredientTypeId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientTypeEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(IngredientTypeEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientTypeEntity.Notes), Table);
}

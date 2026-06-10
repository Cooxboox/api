using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class Ingredients
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.Ingredients), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(IngredientEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(IngredientEntity.EntityId), Table);
  public static readonly ColumnId IngredientId = new(nameof(IngredientEntity.IngredientId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(IngredientEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(IngredientEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(IngredientEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(IngredientEntity.PublishedVersion), Table);
  public static readonly ColumnId Status = new(nameof(IngredientEntity.Status), Table);
}

using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class RecipeTypes
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.RecipeTypes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeTypeEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeTypeEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(RecipeTypeEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeTypeEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeTypeEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeTypeEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(RecipeTypeEntity.EntityId), Table);
  public static readonly ColumnId Icon = new(nameof(RecipeTypeEntity.Icon), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeTypeEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(RecipeTypeEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeTypeEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeTypeEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeTypeEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeTypeEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeTypeId = new(nameof(RecipeTypeEntity.RecipeTypeId), Table);
  public static readonly ColumnId Status = new(nameof(RecipeTypeEntity.Status), Table);
}

using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class RecipeCategories
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.RecipeCategories), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeCategoryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeCategoryEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(RecipeCategoryEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeCategoryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeCategoryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeCategoryEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(RecipeCategoryEntity.EntityId), Table);
  public static readonly ColumnId Icon = new(nameof(RecipeCategoryEntity.Icon), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeCategoryEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(RecipeCategoryEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeCategoryEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeCategoryEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeCategoryEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeCategoryEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeCategoryId = new(nameof(RecipeCategoryEntity.RecipeCategoryId), Table);
  public static readonly ColumnId Status = new(nameof(RecipeCategoryEntity.Status), Table);
}

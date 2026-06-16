using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class Recipes
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.Recipes), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(RecipeEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeEntity.Version), Table);

  public static readonly ColumnId EntityId = new(nameof(RecipeEntity.EntityId), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(RecipeEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeId = new(nameof(RecipeEntity.RecipeId), Table);
  public static readonly ColumnId RecipeTypeId = new(nameof(RecipeEntity.RecipeTypeId), Table);
  public static readonly ColumnId Status = new(nameof(RecipeEntity.Status), Table);
}

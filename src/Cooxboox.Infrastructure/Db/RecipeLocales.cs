using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class RecipeLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.RecipeLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(RecipeLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(RecipeLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(RecipeLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(RecipeLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeId = new(nameof(RecipeLocaleEntity.RecipeId), Table);
  public static readonly ColumnId Slug = new(nameof(RecipeLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(RecipeLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeLocaleEntity.Version), Table);
}

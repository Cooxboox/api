using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class RecipeCategoryLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.RecipeCategoryLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeCategoryLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeCategoryLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(RecipeCategoryLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeCategoryLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(RecipeCategoryLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(RecipeCategoryLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(RecipeCategoryLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeCategoryLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeCategoryLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeCategoryLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeCategoryLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeCategoryId = new(nameof(RecipeCategoryLocaleEntity.RecipeCategoryId), Table);
  public static readonly ColumnId Slug = new(nameof(RecipeCategoryLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(RecipeCategoryLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeCategoryLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeCategoryLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeCategoryLocaleEntity.Version), Table);
}

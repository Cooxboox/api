using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class IngredientCategoryLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.IngredientCategoryLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientCategoryLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientCategoryLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(IngredientCategoryLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId IngredientCategoryId = new(nameof(IngredientCategoryLocaleEntity.IngredientCategoryId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientCategoryLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(IngredientCategoryLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(IngredientCategoryLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(IngredientCategoryLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientCategoryLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(IngredientCategoryLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(IngredientCategoryLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(IngredientCategoryLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId Slug = new(nameof(IngredientCategoryLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(IngredientCategoryLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientCategoryLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientCategoryLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientCategoryLocaleEntity.Version), Table);
}

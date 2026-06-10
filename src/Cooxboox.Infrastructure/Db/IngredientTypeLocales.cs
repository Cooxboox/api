using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class IngredientTypeLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.IngredientTypeLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientTypeLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientTypeLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(IngredientTypeLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId IngredientTypeId = new(nameof(IngredientTypeLocaleEntity.IngredientTypeId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientTypeLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(IngredientTypeLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(IngredientTypeLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(IngredientTypeLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientTypeLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(IngredientTypeLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(IngredientTypeLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(IngredientTypeLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId Slug = new(nameof(IngredientTypeLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(IngredientTypeLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientTypeLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientTypeLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientTypeLocaleEntity.Version), Table);
}

using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class IngredientLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.IngredientLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(IngredientLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(IngredientLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(IngredientLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId IngredientId = new(nameof(IngredientLocaleEntity.IngredientId), Table);
  public static readonly ColumnId KitchenId = new(nameof(IngredientLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(IngredientLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(IngredientLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(IngredientLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(IngredientLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(IngredientLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(IngredientLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(IngredientLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId Slug = new(nameof(IngredientLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(IngredientLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(IngredientLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(IngredientLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(IngredientLocaleEntity.Version), Table);
}

using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class RecipeTypeLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.RecipeTypeLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(RecipeTypeLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(RecipeTypeLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(RecipeTypeLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId KitchenId = new(nameof(RecipeTypeLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(RecipeTypeLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(RecipeTypeLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Name = new(nameof(RecipeTypeLocaleEntity.Name), Table);
  public static readonly ColumnId Notes = new(nameof(RecipeTypeLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(RecipeTypeLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(RecipeTypeLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(RecipeTypeLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId RecipeTypeId = new(nameof(RecipeTypeLocaleEntity.RecipeTypeId), Table);
  public static readonly ColumnId Slug = new(nameof(RecipeTypeLocaleEntity.Slug), Table);
  public static readonly ColumnId Status = new(nameof(RecipeTypeLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(RecipeTypeLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(RecipeTypeLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(RecipeTypeLocaleEntity.Version), Table);
}

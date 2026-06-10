using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class KitchenLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.KitchenLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(KitchenLocaleEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(KitchenLocaleEntity.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(KitchenLocaleEntity.HtmlContent), Table);
  public static readonly ColumnId KitchenId = new(nameof(KitchenLocaleEntity.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(KitchenLocaleEntity.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(KitchenLocaleEntity.MetaDescription), Table);
  public static readonly ColumnId Notes = new(nameof(KitchenLocaleEntity.Notes), Table);
  public static readonly ColumnId PublishedBy = new(nameof(KitchenLocaleEntity.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(KitchenLocaleEntity.PublishedOn), Table);
  public static readonly ColumnId PublishedVersion = new(nameof(KitchenLocaleEntity.PublishedVersion), Table);
  public static readonly ColumnId Status = new(nameof(KitchenLocaleEntity.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(KitchenLocaleEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(KitchenLocaleEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(KitchenLocaleEntity.Version), Table);
}

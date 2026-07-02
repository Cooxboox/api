using Cooxboox.Core.Kitchens;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

public static class KitchenLocales
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.KitchenLocales), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(KitchenLocale.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(KitchenLocale.CreatedOn), Table);
  public static readonly ColumnId HtmlContent = new(nameof(KitchenLocale.HtmlContent), Table);
  public static readonly ColumnId KitchenId = new(nameof(KitchenLocale.KitchenId), Table);
  public static readonly ColumnId Language = new(nameof(KitchenLocale.Language), Table);
  public static readonly ColumnId MetaDescription = new(nameof(KitchenLocale.MetaDescription), Table);
  public static readonly ColumnId PublishedBy = new(nameof(KitchenLocale.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(KitchenLocale.PublishedOn), Table);
  public static readonly ColumnId Status = new(nameof(KitchenLocale.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(KitchenLocale.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(KitchenLocale.UpdatedOn), Table);
}

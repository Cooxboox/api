using Cooxboox.Infrastructure.Configurations;
using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class Kitchens
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.Kitchens), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(KitchenEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(KitchenEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(KitchenEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(KitchenEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(KitchenEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(KitchenEntity.Version), Table);

  public static readonly ColumnId Confidentiality = new(nameof(KitchenEntity.Confidentiality), Table);
  public static readonly ColumnId KitchenId = new(nameof(KitchenEntity.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(KitchenEntity.Name), Table);
  public static readonly ColumnId OwnerId = new(nameof(KitchenEntity.OwnerId), Table);
  public static readonly ColumnId Slug = new(nameof(KitchenEntity.Slug), Table);
  public static readonly ColumnId UniqueId = new(nameof(KitchenEntity.UniqueId), Table);
}

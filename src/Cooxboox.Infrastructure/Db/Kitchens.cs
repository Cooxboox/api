using Cooxboox.Core.Kitchens;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

public static class Kitchens
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.Kitchens), alias: null);

  public static readonly ColumnId Confidentiality = new(nameof(Kitchen.Confidentiality), Table);
  public static readonly ColumnId CreatedBy = new(nameof(Kitchen.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(Kitchen.CreatedOn), Table);
  public static readonly ColumnId Id = new(nameof(Kitchen.Id), Table);
  public static readonly ColumnId KitchenId = new(nameof(Kitchen.KitchenId), Table);
  public static readonly ColumnId Name = new(nameof(Kitchen.Name), Table);
  public static readonly ColumnId Notes = new(nameof(Kitchen.Notes), Table);
  public static readonly ColumnId OwnerId = new(nameof(Kitchen.OwnerId), Table);
  public static readonly ColumnId PublishedBy = new(nameof(Kitchen.PublishedBy), Table);
  public static readonly ColumnId PublishedOn = new(nameof(Kitchen.PublishedOn), Table);
  public static readonly ColumnId Slug = new(nameof(Kitchen.Slug), Table);
  public static readonly ColumnId Status = new(nameof(Kitchen.Status), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(Kitchen.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(Kitchen.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(Kitchen.Version), Table);
}

using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

public static class History
{
  public static readonly TableId Table = new(nameof(CooxbooxContext.History));

  public static readonly ColumnId EntityId = new(nameof(HistoryRecord.EntityId), Table);
  public static readonly ColumnId EntityKind = new(nameof(HistoryRecord.EntityKind), Table);
  public static readonly ColumnId EventData = new(nameof(HistoryRecord.EventData), Table);
  public static readonly ColumnId EventId = new(nameof(HistoryRecord.EventId), Table);
  public static readonly ColumnId EventType = new(nameof(HistoryRecord.EventType), Table);
  public static readonly ColumnId HistoryRecordId = new(nameof(HistoryRecord.HistoryRecordId), Table);
  public static readonly ColumnId KitchenId = new(nameof(HistoryRecord.KitchenId), Table);
  public static readonly ColumnId OccurredOn = new(nameof(HistoryRecord.OccurredOn), Table);
  public static readonly ColumnId UserId = new(nameof(HistoryRecord.UserId), Table);
  public static readonly ColumnId Version = new(nameof(HistoryRecord.Version), Table);
}

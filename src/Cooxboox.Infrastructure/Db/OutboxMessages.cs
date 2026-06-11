using Cooxboox.Infrastructure.Entities;
using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

internal static class OutboxMessages
{
  public static readonly TableId Table = new(nameof(CooxbooxContext.OutboxMessages));

  public static readonly ColumnId Error = new(nameof(OutboxMessageEntity.Error), Table);
  public static readonly ColumnId EventId = new(nameof(OutboxMessageEntity.EventId), Table);
  public static readonly ColumnId OutboxMessageId = new(nameof(OutboxMessageEntity.OutboxMessageId), Table);
  public static readonly ColumnId Status = new(nameof(OutboxMessageEntity.Status), Table);
  public static readonly ColumnId StreamId = new(nameof(OutboxMessageEntity.StreamId), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(OutboxMessageEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(OutboxMessageEntity.Version), Table);
}

using Cooxboox.Core;
using Logitar;
using System.Text.Json;

namespace Cooxboox.Infrastructure;

internal class ChangeRecord // TODO(fpion): configuration
{
  public int ChangeRecordId { get; private set; }
  public Guid EventId { get; private set; }

  public Guid? KitchenId { get; private set; }
  public string EntityKind { get; private set; } = string.Empty;
  public Guid EntityId { get; private set; }

  public long Version { get; private set; }
  public DateTime OccurredOn { get; private set; }
  public Guid? UserId { get; private set; }

  public string EventType { get; private set; } = string.Empty;
  public string EventData { get; private set; } = string.Empty;

  public ChangeRecord(ChangeEvent @event)
  {
    EventId = @event.EventId;

    KitchenId = @event.KitchenId;
    EntityKind = @event.EntityKind;
    EntityId = @event.EntityId;

    Version = @event.Version;
    OccurredOn = @event.OccurredOn;
    UserId = @event.UserId;

    EventType = @event.GetType().GetNamespaceQualifiedName();
    EventData = JsonSerializer.Serialize(@event); // TODO(fpion): implement
  }

  private ChangeRecord()
  {
  }
}

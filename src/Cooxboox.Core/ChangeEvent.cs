namespace Cooxboox.Core;

public abstract class ChangeEvent
{
  public Guid EventId { get; set; } = Guid.NewGuid();

  public Guid? KitchenId { get; set; }
  public string EntityKind { get; set; } = string.Empty;
  public Guid EntityId { get; set; }

  public long Version { get; set; }
  public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
  public Guid? UserId { get; set; }

  public override bool Equals(object? obj) => obj is ChangeEvent @event && @event.EventId == EventId;
  public override int GetHashCode() => EventId.GetHashCode();
  public override string ToString() => $"{base.ToString()} (EventId={EventId})";
}

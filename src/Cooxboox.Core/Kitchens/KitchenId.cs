using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public readonly struct KitchenId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public Guid EntityId { get; }

  public KitchenId(StreamId streamId)
  {
    StreamId = streamId;

    EntityId = Entity.Parse(streamId.Value, Kitchen.EntityKind).Id;
  }

  public KitchenId(string value) : this(new StreamId(value))
  {
  }

  public KitchenId(Guid value) : this(new Entity(Kitchen.EntityKind, value).ToString())
  {
  }

  public static KitchenId NewId() => new(Guid.NewGuid());

  public static bool operator ==(KitchenId left, KitchenId right) => left.Equals(right);
  public static bool operator !=(KitchenId left, KitchenId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is KitchenId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

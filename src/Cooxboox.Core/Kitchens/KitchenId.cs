using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public readonly struct KitchenId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId(StreamId streamId)
  {
    StreamId = streamId;
  }

  public KitchenId(string value) : this(new StreamId(value))
  {
  }

  public KitchenId(Guid value) : this($"Kitchen:{Convert.ToBase64String(value.ToByteArray()).ToUriSafeBase64()}")
  {
  }

  public static KitchenId NewId() => new(Guid.NewGuid());

  public Guid ToGuid() => default; // TODO(fpion): implement

  public static bool operator ==(KitchenId left, KitchenId right) => left.Equals(right);
  public static bool operator !=(KitchenId left, KitchenId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is KitchenId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

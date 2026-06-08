using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes;

public readonly struct IngredientTypeId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public IngredientTypeId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, IngredientType.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public IngredientTypeId(string value) : this(new StreamId(value))
  {
  }

  public IngredientTypeId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(IngredientType.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static IngredientTypeId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(IngredientTypeId left, IngredientTypeId right) => left.Equals(right);
  public static bool operator !=(IngredientTypeId left, IngredientTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is IngredientTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

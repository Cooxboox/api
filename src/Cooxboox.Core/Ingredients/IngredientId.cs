using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients;

public readonly struct IngredientId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public IngredientId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Ingredient.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public IngredientId(string value) : this(new StreamId(value))
  {
  }

  public IngredientId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(Ingredient.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static IngredientId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(IngredientId left, IngredientId right) => left.Equals(right);
  public static bool operator !=(IngredientId left, IngredientId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is IngredientId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

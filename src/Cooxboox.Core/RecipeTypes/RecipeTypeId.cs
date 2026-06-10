using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes;

public readonly struct RecipeTypeId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public RecipeTypeId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, RecipeType.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public RecipeTypeId(string value) : this(new StreamId(value))
  {
  }

  public RecipeTypeId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(RecipeType.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static RecipeTypeId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(RecipeTypeId left, RecipeTypeId right) => left.Equals(right);
  public static bool operator !=(RecipeTypeId left, RecipeTypeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RecipeTypeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

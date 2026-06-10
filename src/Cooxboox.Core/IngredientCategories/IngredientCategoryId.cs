using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories;

public readonly struct IngredientCategoryId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public IngredientCategoryId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, IngredientCategory.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public IngredientCategoryId(string value) : this(new StreamId(value))
  {
  }

  public IngredientCategoryId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(IngredientCategory.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static IngredientCategoryId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(IngredientCategoryId left, IngredientCategoryId right) => left.Equals(right);
  public static bool operator !=(IngredientCategoryId left, IngredientCategoryId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is IngredientCategoryId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

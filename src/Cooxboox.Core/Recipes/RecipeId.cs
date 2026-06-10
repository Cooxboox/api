using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes;

public readonly struct RecipeId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public RecipeId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, Recipe.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public RecipeId(string value) : this(new StreamId(value))
  {
  }

  public RecipeId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(Recipe.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static RecipeId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(RecipeId left, RecipeId right) => left.Equals(right);
  public static bool operator !=(RecipeId left, RecipeId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RecipeId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

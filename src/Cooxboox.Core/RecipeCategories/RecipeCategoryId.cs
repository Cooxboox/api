using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories;

public readonly struct RecipeCategoryId
{
  public StreamId StreamId { get; }
  public string Value => StreamId.Value;

  public KitchenId KitchenId { get; }
  public Guid EntityId { get; }

  public RecipeCategoryId(StreamId streamId)
  {
    StreamId = streamId;

    Entity entity = Entity.Parse(streamId.Value, RecipeCategory.EntityKind);
    KitchenId = entity.KitchenId ?? throw new ArgumentException("The kitchen identifier is required.", nameof(streamId));
    EntityId = entity.Id;
  }

  public RecipeCategoryId(string value) : this(new StreamId(value))
  {
  }

  public RecipeCategoryId(KitchenId kitchenId, Guid entityId)
  {
    StreamId = new StreamId(new Entity(RecipeCategory.EntityKind, entityId, kitchenId).ToString());

    KitchenId = kitchenId;
    EntityId = entityId;
  }

  public static RecipeCategoryId NewId(KitchenId kitchenId) => new(kitchenId, Guid.NewGuid());

  public static bool operator ==(RecipeCategoryId left, RecipeCategoryId right) => left.Equals(right);
  public static bool operator !=(RecipeCategoryId left, RecipeCategoryId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RecipeCategoryId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

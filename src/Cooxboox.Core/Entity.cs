using Cooxboox.Core.Kitchens;
using Logitar;

namespace Cooxboox.Core;

public class Entity
{
  private const char Separator = '|';
  private const char EntitySeparator = ':';

  private readonly string _value;

  public string Kind { get; }
  public Guid Id { get; }
  public KitchenId? KitchenId { get; }

  public Entity(string kind, Guid id, KitchenId? kitchenId = null)
  {
    if (string.IsNullOrWhiteSpace(kind))
    {
      throw new ArgumentException("The kind is required.", nameof(kind));
    }

    Kind = kind.Trim();
    Id = id;
    KitchenId = kitchenId;

    _value = Encode(Kind, Id, KitchenId);
  }
  private static string Encode(string kind, Guid id, KitchenId? kitchenId)
  {
    string entity = string.Join(EntitySeparator, kind, Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64());
    return kitchenId.HasValue ? string.Join(Separator, kitchenId.Value, entity) : entity;
  }

  public static Entity Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException("The entity is not valid.", nameof(value));
    }

    KitchenId? kitchenId = values.Length == 2 ? new(Parse(values.First(), Kitchen.EntityKind).Id) : null;
    Entity entity = Parse(values.Last(), expectedKind);
    return new Entity(entity.Kind, entity.Id, kitchenId);
  }

  public override bool Equals(object? obj) => obj is Entity entity && entity._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

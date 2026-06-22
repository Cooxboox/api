using Cooxboox.Core.Kitchens;
using Logitar;

namespace Cooxboox.Core;

public interface IEntityProvider
{
  Entity GetEntity();
}

public class Entity
{
  private const char Separator = '|';
  private const char EntitySeparator = ':';

  private readonly string _value;

  public string Kind { get; }
  public Guid Id { get; }
  public Guid? KitchenId { get; }

  public Entity(string kind, Guid id, Guid? kitchenId = null)
  {
    if (string.IsNullOrWhiteSpace(kind))
    {
      throw new ArgumentException("The kind is required.", nameof(kind));
    }

    Kind = kind.Trim();
    Id = id;
    KitchenId = kitchenId;

    string value = string.Join(EntitySeparator, Kind, Convert.ToBase64String(Id.ToByteArray()).ToUriSafeBase64());
    _value = kitchenId.HasValue ? string.Join(Separator, new Entity(Kitchen.EntityKind, kitchenId.Value), value) : value;
  }

  public override bool Equals(object? obj) => obj is Entity entity && entity._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

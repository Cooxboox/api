using Cooxboox.Core.Kitchens;
using Logitar;

namespace Cooxboox.Core;

public interface IEntityProvider
{
  Entity Entity { get; }
}

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

    string value = string.Join(EntitySeparator, Kind, Convert.ToBase64String(Id.ToByteArray()).ToUriSafeBase64());
    _value = KitchenId.HasValue ? string.Join(Separator, KitchenId, value) : value;
  }

  public static Entity Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{value}' is not a valid entity.", nameof(value));
    }

    KitchenId? kitchenId = values.Length == 2 ? new(values.First()) : null;

    string[] parts = values.Last().Split(EntitySeparator);
    if (parts.Length != 2)
    {
      throw new ArgumentException($"The value '{values.Last()}' is not a valid entity.", nameof(value));
    }

    string kind = parts.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The entity kind '{kind}' was not expected ({expectedKind}).", nameof(value));
    }
    Guid id = new(Convert.FromBase64String(parts.Last().FromUriSafeBase64()));

    return new Entity(kind, id, kitchenId);
  }

  public override bool Equals(object? obj) => obj is Entity entity && entity._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

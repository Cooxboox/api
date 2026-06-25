using Cooxboox.Core.Kitchens;
using Logitar;

namespace Cooxboox.Core;

public class ResourceIdentifier
{
  private const char Separator = '|';
  private const char ResourceSeparator = ':';

  private readonly string _value;

  public Guid? KitchenId { get; }
  public string Kind { get; }
  public Guid Id { get; }

  public ResourceIdentifier(string kind, Guid id, Guid? kitchenId = null)
  {
    KitchenId = kitchenId;
    Kind = kind.Trim();
    Id = id;

    string value = string.Join(ResourceSeparator, kind, Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64());
    _value = kitchenId.HasValue ? string.Join(Separator, new ResourceIdentifier(Kitchen.ResourceKind, kitchenId.Value), value) : value;
  }

  public static ResourceIdentifier Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The value '{value}' is not a valid resource identifier.", nameof(value));
    }

    Guid? kitchenId = values.Length == 2 ? ResourceIdentifier.Parse(values.First(), Kitchen.ResourceKind).Id : null;

    string[] parts = values.Last().Split(ResourceSeparator);
    if (parts.Length != 2)
    {
      throw new ArgumentException($"The value '{parts.Last()}' is not a valid resource.", nameof(value));
    }

    string kind = parts.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The resource kind '{kind}' was not expected ({expectedKind}).");
    }
    Guid id = new(Convert.FromBase64String(parts.Last().FromUriSafeBase64()));

    return new ResourceIdentifier(kind, id, kitchenId);
  }

  public override bool Equals(object? obj) => obj is ResourceIdentifier identifier && identifier._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

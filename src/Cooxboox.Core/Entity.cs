using Logitar;

namespace Cooxboox.Core;

public interface IEntityProvider
{
  Entity Entity { get; }
}

public class Entity
{
  private const char Separator = ':';

  private readonly string _value;

  public string Kind { get; }
  public Guid Id { get; }

  public Entity(string kind, Guid id)
  {
    if (string.IsNullOrWhiteSpace(kind))
    {
      throw new ArgumentException("The kind is required.", nameof(kind));
    }

    Kind = kind.Trim();
    Id = id;

    _value = string.Join(Separator, Kind, Convert.ToBase64String(Id.ToByteArray()).ToUriSafeBase64());
  }

  public static Entity Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 2)
    {
      throw new ArgumentException($"The value '{value}' is not a valid entity.", nameof(value));
    }

    string kind = values.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The entity kind '{kind}' was not expected ({expectedKind}).", nameof(value));
    }
    Guid id = new(Convert.FromBase64String(values.Last().FromUriSafeBase64()));

    return new Entity(kind, id);
  }

  public override bool Equals(object? obj) => obj is Entity entity && entity._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

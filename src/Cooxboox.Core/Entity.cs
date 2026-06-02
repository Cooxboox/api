using Logitar;

namespace Cooxboox.Core;

public class Entity
{
  private const char Separator = ':';

  private readonly string _value;

  public string Kind { get; }
  public Guid Id { get; }

  public Entity(string kind, Guid id)
  {
    Kind = kind;
    Id = id;

    _value = string.Join(Separator, kind, Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64());
  }

  public static Entity Parse(string value, string? expectedKind = null)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 2)
    {
      throw new ArgumentException($"The entity '{value}' is not valid.", nameof(value));
    }

    string kind = values.First();
    if (expectedKind is not null && expectedKind != kind)
    {
      throw new ArgumentException($"The entity kind '{expectedKind}' was expected, but '{kind}' was received.", nameof(value));
    }

    return new Entity(kind, Guid.Parse(values.Last()));
  }

  public override bool Equals(object? obj) => obj is Entity entity && entity._value == _value;
  public override int GetHashCode() => _value.GetHashCode();
  public override string ToString() => _value;
}

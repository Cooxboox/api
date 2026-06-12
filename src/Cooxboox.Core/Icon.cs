using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core;

public class Icon
{
  public const int MaximumLength = 32;

  public string Value { get; }

  public Icon(string value)
  {
    Value = value.Trim().ToLowerInvariant();
    new Validator().ValidateAndThrow(this);
  }

  public static Icon? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is Icon icon && icon.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<Icon>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Icon();
    }
  }
}

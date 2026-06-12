using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core;

public class Icon
{
  public const int MaximumLength = 32;

  public string Value { get; }

  public Icon(string value)
  {
    Value = Normalize(value);
    new Validator().ValidateAndThrow(this);
  }

  public static string Normalize(string value) => value.Trim().ToLowerInvariant(); // TODO(fpion): why are we normalizing?
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

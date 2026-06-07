using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core;

public class Notes
{
  public string Value { get; }

  public Notes(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Notes? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is Notes notes && notes.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<Notes>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Notes();
    }
  }
}

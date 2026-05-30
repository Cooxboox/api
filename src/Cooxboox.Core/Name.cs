using FluentValidation;

namespace Cooxboox.Core;

public class Name
{
  public string Value { get; }

  public Name(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Name? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private class Validator : AbstractValidator<Name>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty(); // TODO(fpion): implement
    }
  }
}

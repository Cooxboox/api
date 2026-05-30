using FluentValidation;

namespace Cooxboox.Core;

public class Description
{
  public string Value { get; }

  public Description(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static Description? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private class Validator : AbstractValidator<Description>
  {
    public Validator()
    {
      RuleFor(x => x.Value).NotEmpty(); // TODO(fpion): implement
    }
  }
}

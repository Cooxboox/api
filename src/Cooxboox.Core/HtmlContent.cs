using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core;

public class HtmlContent
{
  public string Value { get; }

  public HtmlContent(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static HtmlContent? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private class Validator : AbstractValidator<HtmlContent>
  {
    public Validator()
    {
      RuleFor(x => x.Value).HtmlContent();
    }
  }
}

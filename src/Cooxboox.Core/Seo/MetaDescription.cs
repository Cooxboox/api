using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Seo;

public class MetaDescription
{
  public const int MaximumLength = 160;

  public string Value { get; }

  public MetaDescription(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public static MetaDescription? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  public override bool Equals(object? obj) => obj is MetaDescription metaDescription && metaDescription.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<MetaDescription>
  {
    public Validator()
    {
      RuleFor(x => x.Value).MetaDescription();
    }
  }
}

using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Seo;

public class Slug
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public Slug(string value)
  {
    Value = Normalize(value);
    new Validator().ValidateAndThrow(this);
  }

  public static string Normalize(string value) => value.Trim().ToLowerInvariant();

  public static Slug? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new(value);

  private class Validator : AbstractValidator<Slug>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Slug();
    }
  }
}

using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Seo;

public class Slug
{
  public const int MaximumLength = 100;

  public string Value { get; }

  public Slug(string value)
  {
    Value = Normalize(value);
  }

  public static string Normalize(string value) => value.Trim().ToLowerInvariant();
  public static Slug? TryCreate(string? value) => string.IsNullOrWhiteSpace(value) ? null : new Slug(value);

  public override bool Equals(object? obj) => obj is Slug slug && slug.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;

  private class Validator : AbstractValidator<Slug>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Slug();
    }
  }
}

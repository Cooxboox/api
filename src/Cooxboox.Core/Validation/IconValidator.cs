using Cooxboox.Core.Seo;
using FluentValidation;
using FluentValidation.Validators;

namespace Cooxboox.Core.Validation;

internal class IconValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _knownKinds = new(["emoji"]);

  public string Name { get; } = "IconValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' is not a valid icon.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    string[] parts = value.Split(':');
    return parts.Length == 2 && _knownKinds.Contains(parts.First().ToLowerInvariant()) && Slug.IsValid(parts.Last());
  }
}

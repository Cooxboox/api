using Cooxboox.Core.Localization;
using FluentValidation;
using FluentValidation.Validators;

namespace Cooxboox.Core.Validation;

internal class LanguageValidator<T> : IPropertyValidator<T, string>
{
  private readonly HashSet<string> _knownCodes = Languages.All.ToHashSet();

  public string Name { get; } = "LanguageValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return $"'{{PropertyName}}' must be one of the following: {string.Join(", ", _knownCodes)}.";
  }

  public bool IsValid(ValidationContext<T> context, string value) => _knownCodes.Contains(value);
}

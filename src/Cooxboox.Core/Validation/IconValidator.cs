using FluentValidation;
using FluentValidation.Validators;

namespace Cooxboox.Core.Validation;

internal class IconValidator<T> : IPropertyValidator<T, string>
{
  public string Name { get; } = "IconValidator";

  public string GetDefaultMessageTemplate(string errorCode)
  {
    return "'{PropertyName}' is not a valid icon.";
  }

  public bool IsValid(ValidationContext<T> context, string value)
  {
    string[] parts = value.Split(':');
    return parts.Length == 2 && parts.All(part => part.IsKebabCase()); // TODO(fpion): first part should be a known icon kind!
  }
}

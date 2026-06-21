using FluentValidation;

namespace Cooxboox.Core.Validation;

internal static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Constants.NameMaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Notes<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Constants.SlugMaximumLength).SetValidator(new SlugValidator<T>());
  }
}

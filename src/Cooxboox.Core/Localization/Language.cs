using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Localization;

public class Language
{
  public const int MaximumLength = 2;

  public static readonly Language English = new(CultureInfo.GetCultureInfo(Languages.English));
  public static readonly Language French = new(CultureInfo.GetCultureInfo(Languages.French));

  public static IReadOnlyCollection<Language> All { get; } = new Language[] { English, French }.ToList().AsReadOnly();
  public static readonly Language Default = new(CultureInfo.GetCultureInfo(Languages.Default));

  public CultureInfo Culture { get; }
  public string Code { get; }

  public bool IsDefault => Default.Equals(this);
  public bool IsEnglish => English.Equals(this);
  public bool IsFrench => French.Equals(this);

  public Language(string code)
  {
    Code = code.Trim().ToLowerInvariant();
    new Validator().ValidateAndThrow(this);

    Culture = CultureInfo.GetCultureInfo(Code);
  }
  private Language(CultureInfo culture) : this(culture.Name)
  {
  }

  public override bool Equals(object? obj) => obj is Language language && language.Code == Code;
  public override int GetHashCode() => Code.GetHashCode();
  public override string ToString() => $"{Culture.DisplayName} ({Code})";

  private class Validator : AbstractValidator<Language>
  {
    public Validator()
    {
      RuleFor(x => x.Code).Language();
    }
  }
}

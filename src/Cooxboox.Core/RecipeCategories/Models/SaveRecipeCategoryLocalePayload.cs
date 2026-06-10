using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.RecipeCategories.Models;

public record SaveRecipeCategoryLocalePayload
{
  public string Name { get; set; }
  public string? Slug { get; set; }
  public string? MetaDescription { get; set; }
  public string? HtmlContent { get; set; }
  public string? Notes { get; set; }

  public SaveRecipeCategoryLocalePayload() : this(string.Empty)
  {
  }

  public SaveRecipeCategoryLocalePayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<SaveRecipeCategoryLocalePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Slug), () => RuleFor(x => x.Slug!).Slug());
      When(x => !string.IsNullOrWhiteSpace(x.MetaDescription), () => RuleFor(x => x.MetaDescription!).MetaDescription());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

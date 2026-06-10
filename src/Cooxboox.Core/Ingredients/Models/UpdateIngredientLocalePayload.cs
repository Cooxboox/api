using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Ingredients.Models;

public record UpdateIngredientLocalePayload
{
  public string? Name { get; set; }
  public Optional<string>? Slug { get; set; }
  public Optional<string>? MetaDescription { get; set; }
  public Optional<string>? HtmlContent { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateIngredientLocalePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Slug?.Value), () => RuleFor(x => x.Slug!.Value!).Slug());
      When(x => !string.IsNullOrWhiteSpace(x.MetaDescription?.Value), () => RuleFor(x => x.MetaDescription!.Value!).MetaDescription());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

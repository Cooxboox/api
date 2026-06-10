using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record SaveKitchenLocalePayload
{
  public string? MetaDescription { get; set; }
  public string? HtmlContent { get; set; }
  public string? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<SaveKitchenLocalePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.MetaDescription), () => RuleFor(x => x.MetaDescription!).MetaDescription());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent), () => RuleFor(x => x.HtmlContent!).HtmlContent());
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

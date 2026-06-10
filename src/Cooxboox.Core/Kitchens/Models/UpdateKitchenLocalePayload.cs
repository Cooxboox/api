using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record UpdateKitchenLocalePayload
{
  public Optional<string>? MetaDescription { get; set; }
  public Optional<string>? HtmlContent { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateKitchenLocalePayload>
  {
    public Validator()
    {
      When(x => !string.IsNullOrWhiteSpace(x.MetaDescription?.Value), () => RuleFor(x => x.MetaDescription!.Value!).MetaDescription());
      When(x => !string.IsNullOrWhiteSpace(x.HtmlContent?.Value), () => RuleFor(x => x.HtmlContent!.Value!).HtmlContent());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

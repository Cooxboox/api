using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record UpdateKitchenPayload
{
  public Confidentiality? Confidentiality { get; set; }

  public string? Name { get; set; }
  public Optional<string>? Slug { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateKitchenPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Confidentiality).IsInEnum();

      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Slug?.Value), () => RuleFor(x => x.Slug!.Value!).Slug());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

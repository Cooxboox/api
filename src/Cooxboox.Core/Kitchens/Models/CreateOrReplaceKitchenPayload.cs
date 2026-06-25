using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record CreateOrReplaceKitchenPayload
{
  public Confidentiality Confidentiality { get; set; }

  public string Name { get; set; } = string.Empty;
  public string? Slug { get; set; }
  public string? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceKitchenPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Confidentiality).IsInEnum();

      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Slug), () => RuleFor(x => x.Slug!).Slug());
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

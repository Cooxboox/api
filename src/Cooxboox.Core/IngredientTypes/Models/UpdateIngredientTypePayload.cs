using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.IngredientTypes.Models;

public record UpdateIngredientTypePayload
{
  public string? Name { get; set; }
  public Optional<string>? Icon { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateIngredientTypePayload>
  {
    public Validator()
    {

      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Icon?.Value), () => RuleFor(x => x.Icon!.Value!).Icon());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

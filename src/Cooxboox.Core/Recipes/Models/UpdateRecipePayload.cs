using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Recipes.Models;

public record UpdateRecipePayload
{
  public string? Name { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateRecipePayload>
  {
    public Validator()
    {

      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

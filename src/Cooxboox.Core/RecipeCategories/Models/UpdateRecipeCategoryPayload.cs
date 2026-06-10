using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.RecipeCategories.Models;

public record UpdateRecipeCategoryPayload
{
  public string? Name { get; set; }
  public Optional<string>? Notes { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateRecipeCategoryPayload>
  {
    public Validator()
    {

      When(x => !string.IsNullOrWhiteSpace(x.Name), () => RuleFor(x => x.Name!).Name());
      When(x => !string.IsNullOrWhiteSpace(x.Notes?.Value), () => RuleFor(x => x.Notes!.Value!).Notes());
    }
  }
}

using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Ingredients.Models;

public record CreateOrReplaceIngredientPayload
{
  public string Name { get; set; }
  public string? Notes { get; set; }

  public CreateOrReplaceIngredientPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceIngredientPayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceIngredientPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

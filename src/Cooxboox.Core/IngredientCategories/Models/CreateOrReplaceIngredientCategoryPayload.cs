using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.IngredientCategories.Models;

public record CreateOrReplaceIngredientCategoryPayload
{
  public string Name { get; set; }
  public string? Notes { get; set; }

  public CreateOrReplaceIngredientCategoryPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceIngredientCategoryPayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceIngredientCategoryPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.RecipeCategories.Models;

public record CreateOrReplaceRecipeCategoryPayload
{
  public string Name { get; set; }
  public string? Notes { get; set; }

  public CreateOrReplaceRecipeCategoryPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceRecipeCategoryPayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceRecipeCategoryPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

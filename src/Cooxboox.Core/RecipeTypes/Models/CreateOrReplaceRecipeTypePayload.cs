using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.RecipeTypes.Models;

public record CreateOrReplaceRecipeTypePayload
{
  public string Name { get; set; }
  public string? Icon { get; set; }
  public string? Notes { get; set; }

  public CreateOrReplaceRecipeTypePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceRecipeTypePayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceRecipeTypePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Icon), () => RuleFor(x => x.Icon!).Icon());
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

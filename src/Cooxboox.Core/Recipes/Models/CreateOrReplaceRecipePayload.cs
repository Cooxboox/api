using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Recipes.Models;

public record CreateOrReplaceRecipePayload
{
  public string Name { get; set; }
  public string? Notes { get; set; }

  public Guid? TypeId { get; set; }

  public CreateOrReplaceRecipePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceRecipePayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceRecipePayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
      When(x => !string.IsNullOrWhiteSpace(x.Notes), () => RuleFor(x => x.Notes!).Notes());
    }
  }
}

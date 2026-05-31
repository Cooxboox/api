using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record CreateOrReplaceKitchenPayload
{
  public string Name { get; set; } = string.Empty;

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceKitchenPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
    }
  }
}

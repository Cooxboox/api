using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record CreateOrReplaceKitchenPayload
{
  public string Name { get; set; }

  public CreateOrReplaceKitchenPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceKitchenPayload(string name)
  {
    Name = name;
  }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<CreateOrReplaceKitchenPayload>
  {
    public Validator()
    {
      RuleFor(x => x.Name).Name();
    }
  }
}

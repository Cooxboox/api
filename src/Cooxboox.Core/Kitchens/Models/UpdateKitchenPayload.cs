using Cooxboox.Core.Validation;
using FluentValidation;

namespace Cooxboox.Core.Kitchens.Models;

public record UpdateKitchenPayload
{
  public string? Name { get; set; }

  public void Validate() => new Validator().ValidateAndThrow(this);

  private class Validator : AbstractValidator<UpdateKitchenPayload>
  {
    public Validator()
    {
      When(x => x.Name is not null, () => RuleFor(x => x.Name!).Name());
    }
  }
}

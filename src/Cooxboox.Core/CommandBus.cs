using Cooxboox.Core.Identity;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using FluentValidation;
using Logitar.CQRS;

namespace Cooxboox.Core;

internal class CommandBus : Logitar.CQRS.CommandBus
{
  public CommandBus(IServiceProvider serviceProvider) : base(serviceProvider)
  {
  }

  protected override bool ShouldRetry<TResult>(ICommand<TResult> command, Exception exception)
    => exception is not AuthenticationFlowNotAllowedException
    && exception is not IdentityException
    && exception is not NotFoundException
    && exception is not PermissionDeniedException
    && exception is not ValidationException;
}

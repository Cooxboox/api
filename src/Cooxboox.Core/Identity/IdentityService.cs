using Cooxboox.Core.Identity.Commands;
using Cooxboox.Core.Identity.Models;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Identity;

public interface IIdentityService
{
  Task<SignInAccountResult> SignInAsync(SignInAccountPayload payload, CancellationToken cancellationToken = default);
}

internal class IdentityService : IIdentityService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IIdentityService, IdentityService>();
    services.AddTransient<ICommandHandler<SignInAccountCommand, SignInAccountResult>, SignInAccountCommandHandler>();
  }

  private readonly ICommandBus _commandBus;

  public IdentityService(ICommandBus commandBus)
  {
    _commandBus = commandBus;
  }

  public async Task<SignInAccountResult> SignInAsync(SignInAccountPayload payload, CancellationToken cancellationToken)
  {
    SignInAccountCommand command = new(payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public record MigrateDatabaseCommand : ICommand;

internal class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabaseCommand, Unit>
{
  private readonly CooxbooxContext _cooxboox;

  public MigrateDatabaseCommandHandler(CooxbooxContext cooxboox)
  {
    _cooxboox = cooxboox;
  }

  public async Task<Unit> HandleAsync(MigrateDatabaseCommand command, CancellationToken cancellationToken)
  {
    await _cooxboox.Database.MigrateAsync(cancellationToken);

    return Unit.Value;
  }
}

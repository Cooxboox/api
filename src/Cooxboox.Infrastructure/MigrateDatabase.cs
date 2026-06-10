using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public record MigrateDatabaseCommand : ICommand;

internal class MigrateDatabaseCommandHandler : ICommandHandler<MigrateDatabaseCommand, Unit>
{
  private readonly CooxbooxContext _cooxboox;
  private readonly EventContext _events;

  public MigrateDatabaseCommandHandler(CooxbooxContext cooxboox, EventContext events)
  {
    _cooxboox = cooxboox;
    _events = events;
  }

  public async Task<Unit> HandleAsync(MigrateDatabaseCommand _, CancellationToken cancellationToken)
  {
    await _events.Database.MigrateAsync(cancellationToken);
    await _cooxboox.Database.MigrateAsync(cancellationToken);

    return Unit.Value;
  }
}

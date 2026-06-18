using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UpdateKitchenCommand(Guid Id, UpdateKitchenPayload Payload) : ICommand<KitchenModel?>;

internal class UpdateKitchenCommandHandler : ICommandHandler<UpdateKitchenCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IDbContext _database;
  private readonly IKitchenManager _kitchenManager;
  private readonly IPermissionService _permissionService;

  public UpdateKitchenCommandHandler(
    IContext context,
    IDbContext database,
    IKitchenManager kitchenManager,
    IPermissionService permissionService)
  {
    _context = context;
    _database = database;
    _kitchenManager = kitchenManager;
    _permissionService = permissionService;
  }

  public async Task<KitchenModel?> HandleAsync(UpdateKitchenCommand command, CancellationToken cancellationToken)
  {
    UpdateKitchenPayload payload = command.Payload;
    payload.Validate();

    Kitchen? kitchen = await _database.Kitchens.SingleOrDefaultAsync(x => x.EntityId == command.Id, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

    kitchen.Update(
      string.IsNullOrWhiteSpace(payload.Name) ? kitchen.Name : payload.Name,
      payload.Slug is null ? kitchen.Slug : payload.Slug.Value,
      payload.Notes is null ? kitchen.Notes : payload.Notes.Value,
      _context.UserId);

    await _kitchenManager.EnsureUniticityAsync(kitchen, cancellationToken);

    // TODO(fpion): audit event

    await _database.SaveChangesAsync(cancellationToken);

    return null; // TODO(fpion): map
  }
}

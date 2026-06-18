using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Core.Kitchens.Commands;

internal record CreateOrReplaceKitchenCommand(CreateOrReplaceKitchenPayload Payload, Guid? Id) : ICommand<CreateOrReplaceKitchenResult>;

internal class CreateOrReplaceKitchenCommandHandler : ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>
{
  private readonly IContext _context;
  private readonly IDbContext _database;
  private readonly IKitchenManager _kitchenManager;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceKitchenCommandHandler(IContext context, IDbContext database, IKitchenManager kitchenManager, IPermissionService permissionService)
  {
    _context = context;
    _database = database;
    _kitchenManager = kitchenManager;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceKitchenResult> HandleAsync(CreateOrReplaceKitchenCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenPayload payload = command.Payload;
    payload.Validate();

    Guid? kitchenId = null;
    Kitchen? kitchen = null;
    if (command.Id.HasValue)
    {
      kitchenId = command.Id.Value;
      kitchen = await _database.Kitchens.SingleOrDefaultAsync(x => x.EntityId == kitchenId, cancellationToken);
    }

    bool created = false;
    if (kitchen is null)
    {
      await _permissionService.CheckAsync(Actions.CreateKitchen, cancellationToken);

      kitchen = new Kitchen(_context.UserId, payload.Name, kitchenId, Confidentiality.Private, payload.Slug, payload.Notes);
      _database.Kitchens.Add(kitchen);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

      kitchen.Update(_context.UserId, payload.Name, payload.Slug, payload.Notes);
    }

    await _kitchenManager.EnsureUniticityAsync(kitchen, cancellationToken);

    // TODO(fpion): audit event

    await _database.SaveChangesAsync(cancellationToken);

    KitchenModel model = null!; // TODO(fpion): map
    return new CreateOrReplaceKitchenResult(model, created);
  }
}

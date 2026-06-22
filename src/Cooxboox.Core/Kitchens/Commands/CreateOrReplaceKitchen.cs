using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record CreateOrReplaceKitchenCommand(CreateOrReplaceKitchenPayload Payload, Guid? Id) : ICommand<CreateOrReplaceKitchenResult>;

internal class CreateOrReplaceKitchenCommandHandler : ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>
{
  private readonly IContext _context;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceKitchenCommandHandler(IContext context, IKitchenRepository kitchenRepository, IPermissionService permissionService)
  {
    _context = context;
    _kitchenRepository = kitchenRepository;
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
      kitchen = await _kitchenRepository.LoadAsync(kitchenId.Value, cancellationToken);
    }

    bool created = false;
    if (kitchen is null)
    {
      await _permissionService.CheckAsync(Actions.CreateKitchen, cancellationToken);

      kitchen = new Kitchen(_context.UserId, payload.Name, kitchenId, Confidentiality.Private, payload.Slug, payload.Notes);
      _kitchenRepository.Add(kitchen);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

      KitchenUpdated @event = kitchen.Update(payload.Name, payload.Slug, payload.Notes, _context.UserId);
      _kitchenRepository.Update(kitchen, @event);
    }

    await _kitchenRepository.EnsureUnicityAsync(kitchen, cancellationToken);

    await _context.SaveChangesAsync(cancellationToken);

    KitchenModel model = await _kitchenRepository.ReadAsync(kitchen, cancellationToken);
    return new CreateOrReplaceKitchenResult(model, created);
  }
}

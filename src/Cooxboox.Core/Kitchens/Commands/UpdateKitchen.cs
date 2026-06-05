using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UpdateKitchenCommand(Guid Id, UpdateKitchenPayload Payload) : ICommand<KitchenModel?>;

internal class UpdateKitchenCommandHandler : ICommandHandler<UpdateKitchenCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public UpdateKitchenCommandHandler(IContext context, IKitchenQuerier kitchenQuerier, IKitchenRepository kitchenRepository, IPermissionService permissionService)
  {
    _context = context;
    _kitchenQuerier = kitchenQuerier;
    _kitchenRepository = kitchenRepository;
    _permissionService = permissionService;
  }

  public async Task<KitchenModel?> HandleAsync(UpdateKitchenCommand command, CancellationToken cancellationToken)
  {
    UpdateKitchenPayload payload = command.Payload;
    payload.Validate();

    KitchenId kitchenId = new(command.Id);
    Kitchen? kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

    Name name = Name.TryCreate(payload.Name) ?? kitchen.Name;

    kitchen.Update(name, _context.ActorId);

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

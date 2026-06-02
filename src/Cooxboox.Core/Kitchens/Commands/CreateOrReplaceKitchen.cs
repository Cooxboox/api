using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record CreateOrReplaceKitchenCommand(CreateOrReplaceKitchenPayload Payload, Guid? Id) : ICommand<CreateOrReplaceKitchenResult>;

internal class CreateOrReplaceKitchenCommandHandler : ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceKitchenCommandHandler(
    IContext context,
    IKitchenQuerier kitchenQuerier,
    IKitchenRepository kitchenRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _kitchenQuerier = kitchenQuerier;
    _kitchenRepository = kitchenRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceKitchenResult> HandleAsync(CreateOrReplaceKitchenCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenPayload payload = command.Payload;
    payload.Validate();

    KitchenId kitchenId = KitchenId.NewId();
    Kitchen? kitchen = null;
    if (command.Id.HasValue)
    {
      kitchenId = new KitchenId(command.Id.Value);
      kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    }

    Name name = new(payload.Name);

    bool created = false;
    if (kitchen is null)
    {
      await _permissionService.CheckAsync(Actions.CreateKitchen, cancellationToken);

      kitchen = new Kitchen(_context.UserId, name, kitchenId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

      kitchen.Rename(name, _context.ActorId);
    }

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    KitchenModel model = await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
    return new CreateOrReplaceKitchenResult(model, created);
  }
}

using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UpdateKitchenCommand(Guid Id, UpdateKitchenPayload Payload) : ICommand<KitchenModel?>;

internal class UpdateKitchenCommandHandler : ICommandHandler<UpdateKitchenCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public UpdateKitchenCommandHandler(
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

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      kitchen.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Slug is not null)
    {
      kitchen.SetSlug(Slug.TryCreate(payload.Slug.Value), actorId);
    }
    if (payload.Notes is not null)
    {
      kitchen.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

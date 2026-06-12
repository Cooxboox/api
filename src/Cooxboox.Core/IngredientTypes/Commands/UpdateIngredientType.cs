using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Commands;

internal record UpdateIngredientTypeCommand(Guid Id, UpdateIngredientTypePayload Payload) : ICommand<IngredientTypeModel?>;

internal class UpdateIngredientTypeCommandHandler : ICommandHandler<UpdateIngredientTypeCommand, IngredientTypeModel?>
{
  private readonly IContext _context;
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientTypeCommandHandler(
    IContext context,
    IIngredientTypeQuerier ingredientTypeQuerier,
    IIngredientTypeRepository ingredientTypeRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientTypeQuerier = ingredientTypeQuerier;
    _ingredientTypeRepository = ingredientTypeRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientTypeModel?> HandleAsync(UpdateIngredientTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateIngredientTypePayload payload = command.Payload;
    payload.Validate();

    IngredientTypeId ingredientTypeId = new(_context.KitchenId, command.Id);
    IngredientType? ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken);
    if (ingredientType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientType, cancellationToken);

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      ingredientType.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Icon is not null)
    {
      ingredientType.SetIcon(Icon.TryCreate(payload.Icon.Value), actorId);
    }
    if (payload.Notes is not null)
    {
      ingredientType.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    await _ingredientTypeRepository.SaveAsync(ingredientType, cancellationToken);

    return await _ingredientTypeQuerier.ReadAsync(ingredientType, cancellationToken);
  }
}

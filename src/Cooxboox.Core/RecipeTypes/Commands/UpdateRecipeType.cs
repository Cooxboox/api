using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record UpdateRecipeTypeCommand(Guid Id, UpdateRecipeTypePayload Payload) : ICommand<RecipeTypeModel?>;

internal class UpdateRecipeTypeCommandHandler : ICommandHandler<UpdateRecipeTypeCommand, RecipeTypeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public UpdateRecipeTypeCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeTypeQuerier recipeTypeQuerier,
    IRecipeTypeRepository recipeTypeRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeTypeQuerier = recipeTypeQuerier;
    _recipeTypeRepository = recipeTypeRepository;
  }

  public async Task<RecipeTypeModel?> HandleAsync(UpdateRecipeTypeCommand command, CancellationToken cancellationToken)
  {
    UpdateRecipeTypePayload payload = command.Payload;
    payload.Validate();

    RecipeTypeId recipeTypeId = new(_context.KitchenId, command.Id);
    RecipeType? recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    if (recipeType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeType, cancellationToken);

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      recipeType.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Notes is not null)
    {
      recipeType.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    return await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
  }
}

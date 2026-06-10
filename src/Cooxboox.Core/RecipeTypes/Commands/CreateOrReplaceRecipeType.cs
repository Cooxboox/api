using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record CreateOrReplaceRecipeTypeCommand(CreateOrReplaceRecipeTypePayload Payload, Guid? Id) : ICommand<CreateOrReplaceRecipeTypeResult>;

internal class CreateOrReplaceRecipeTypeCommandHandler : ICommandHandler<CreateOrReplaceRecipeTypeCommand, CreateOrReplaceRecipeTypeResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public CreateOrReplaceRecipeTypeCommandHandler(
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

  public async Task<CreateOrReplaceRecipeTypeResult> HandleAsync(CreateOrReplaceRecipeTypeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeTypePayload payload = command.Payload;
    payload.Validate();

    RecipeTypeId recipeTypeId = RecipeTypeId.NewId(_context.KitchenId);
    RecipeType? recipeType = null;
    if (command.Id.HasValue)
    {
      recipeTypeId = new RecipeTypeId(recipeTypeId.KitchenId, command.Id.Value);
      recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);
    Notes? notes = Notes.TryCreate(payload.Notes);

    bool created = false;
    if (recipeType is null)
    {
      await _permissionService.CheckAsync(Actions.CreateRecipeType, cancellationToken);

      recipeType = new RecipeType(recipeTypeId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, recipeType, cancellationToken);
    }

    recipeType.Update(name, notes, actorId);

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    RecipeTypeModel model = await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
    return new CreateOrReplaceRecipeTypeResult(model, created);
  }
}

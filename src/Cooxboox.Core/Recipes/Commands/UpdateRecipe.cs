using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Core.RecipeTypes;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Commands;

internal record UpdateRecipeCommand(Guid Id, UpdateRecipePayload Payload) : ICommand<RecipeModel?>;

internal class UpdateRecipeCommandHandler : ICommandHandler<UpdateRecipeCommand, RecipeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeQuerier _recipeQuerier;
  private readonly IRecipeRepository _recipeRepository;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public UpdateRecipeCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeQuerier recipeQuerier,
    IRecipeRepository recipeRepository,
    IRecipeTypeRepository recipeTypeRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeQuerier = recipeQuerier;
    _recipeRepository = recipeRepository;
    _recipeTypeRepository = recipeTypeRepository;
  }

  public async Task<RecipeModel?> HandleAsync(UpdateRecipeCommand command, CancellationToken cancellationToken)
  {
    UpdateRecipePayload payload = command.Payload;
    payload.Validate();

    RecipeId recipeId = new(_context.KitchenId, command.Id);
    Recipe? recipe = await _recipeRepository.LoadAsync(recipeId, cancellationToken);
    if (recipe is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipe, cancellationToken);

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      recipe.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Notes is not null)
    {
      recipe.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    if (payload.TypeId is not null)
    {
      RecipeType? recipeType = null;
      if (payload.TypeId.Value.HasValue)
      {
        RecipeTypeId recipeTypeId = new(recipeId.KitchenId, payload.TypeId.Value.Value);
        recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken)
          ?? throw new EntityNotFoundException(new Entity(RecipeType.EntityKind, recipeTypeId.EntityId, recipeTypeId.KitchenId), nameof(payload.TypeId));
      }
      recipe.SetType(recipeType, actorId);
    }

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

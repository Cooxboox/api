using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
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

  public UpdateRecipeCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeQuerier recipeQuerier,
    IRecipeRepository recipeRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeQuerier = recipeQuerier;
    _recipeRepository = recipeRepository;
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

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

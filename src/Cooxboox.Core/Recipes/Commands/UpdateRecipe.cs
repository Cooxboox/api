using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Logitar.CQRS;

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

    Name name = Name.TryCreate(payload.Name) ?? recipe.Name;
    Notes? notes = payload.Notes is null ? recipe.Notes : Notes.TryCreate(payload.Notes.Value);

    recipe.Update(name, notes, _context.ActorId);

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

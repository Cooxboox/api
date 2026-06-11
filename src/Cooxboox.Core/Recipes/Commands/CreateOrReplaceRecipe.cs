using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Commands;

internal record CreateOrReplaceRecipeCommand(CreateOrReplaceRecipePayload Payload, Guid? Id) : ICommand<CreateOrReplaceRecipeResult>;

internal class CreateOrReplaceRecipeCommandHandler : ICommandHandler<CreateOrReplaceRecipeCommand, CreateOrReplaceRecipeResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeQuerier _recipeQuerier;
  private readonly IRecipeRepository _recipeRepository;

  public CreateOrReplaceRecipeCommandHandler(
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

  public async Task<CreateOrReplaceRecipeResult> HandleAsync(CreateOrReplaceRecipeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipePayload payload = command.Payload;
    payload.Validate();

    RecipeId recipeId = RecipeId.NewId(_context.KitchenId);
    Recipe? recipe = null;
    if (command.Id.HasValue)
    {
      recipeId = new RecipeId(recipeId.KitchenId, command.Id.Value);
      recipe = await _recipeRepository.LoadAsync(recipeId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);

    bool created = false;
    if (recipe is null)
    {
      await _permissionService.CheckAsync(Actions.CreateRecipe, cancellationToken);

      recipe = new Recipe(recipeId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, recipe, cancellationToken);

      recipe.Rename(name, actorId);
    }

    recipe.Annotate(Notes.TryCreate(payload.Notes), actorId);

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    RecipeModel model = await _recipeQuerier.ReadAsync(recipe, cancellationToken);
    return new CreateOrReplaceRecipeResult(model, created);
  }
}

using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Recipes.Commands;

internal record SaveRecipeLocaleCommand(Guid Id, string Language, SaveRecipeLocalePayload Payload) : ICommand<RecipeModel?>;

internal class SaveRecipeLocaleCommandHandler : ICommandHandler<SaveRecipeLocaleCommand, RecipeModel?>
{
  private readonly IContext _context;
  private readonly IRecipeQuerier _recipeQuerier;
  private readonly IRecipeRepository _recipeRepository;
  private readonly IPermissionService _permissionService;

  public SaveRecipeLocaleCommandHandler(
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

  public async Task<RecipeModel?> HandleAsync(SaveRecipeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveRecipeLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeId recipeId = new(_context.KitchenId, command.Id);
    Recipe? recipe = await _recipeRepository.LoadAsync(recipeId, cancellationToken);
    if (recipe is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipe, cancellationToken);

    RecipeLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    recipe.SetLocale(language, locale, _context.ActorId);

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

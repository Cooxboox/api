using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Recipes.Commands;

internal record UpdateRecipeLocaleCommand(Guid Id, string Language, UpdateRecipeLocalePayload Payload) : ICommand<RecipeModel?>;

internal class UpdateRecipeLocaleCommandHandler : ICommandHandler<UpdateRecipeLocaleCommand, RecipeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeQuerier _recipeQuerier;
  private readonly IRecipeRepository _recipeRepository;

  public UpdateRecipeLocaleCommandHandler(
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

  public async Task<RecipeModel?> HandleAsync(UpdateRecipeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateRecipeLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeId recipeId = new(_context.KitchenId, command.Id);
    Recipe? recipe = await _recipeRepository.LoadAsync(recipeId, cancellationToken);
    if (recipe is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipe, cancellationToken);

    RecipeLocale locale = recipe.FindLocale(language);
    locale = new RecipeLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    recipe.SetLocale(language, locale, _context.ActorId);

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

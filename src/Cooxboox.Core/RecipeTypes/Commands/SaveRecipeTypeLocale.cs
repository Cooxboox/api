using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record SaveRecipeTypeLocaleCommand(Guid Id, string Language, SaveRecipeTypeLocalePayload Payload) : ICommand<RecipeTypeModel?>;

internal class SaveRecipeTypeLocaleCommandHandler : ICommandHandler<SaveRecipeTypeLocaleCommand, RecipeTypeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public SaveRecipeTypeLocaleCommandHandler(
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

  public async Task<RecipeTypeModel?> HandleAsync(SaveRecipeTypeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveRecipeTypeLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeTypeId recipeTypeId = new(_context.KitchenId, command.Id);
    RecipeType? recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    if (recipeType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeType, cancellationToken);

    RecipeTypeLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    recipeType.SetLocale(language, locale, _context.ActorId);

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    return await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
  }
}

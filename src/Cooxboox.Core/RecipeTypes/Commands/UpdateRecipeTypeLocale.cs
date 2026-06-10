using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record UpdateRecipeTypeLocaleCommand(Guid Id, string Language, UpdateRecipeTypeLocalePayload Payload) : ICommand<RecipeTypeModel?>;

internal class UpdateRecipeTypeLocaleCommandHandler : ICommandHandler<UpdateRecipeTypeLocaleCommand, RecipeTypeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public UpdateRecipeTypeLocaleCommandHandler(
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

  public async Task<RecipeTypeModel?> HandleAsync(UpdateRecipeTypeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateRecipeTypeLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeTypeId recipeTypeId = new(_context.KitchenId, command.Id);
    RecipeType? recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    if (recipeType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeType, cancellationToken);

    RecipeTypeLocale locale = recipeType.FindLocale(language);
    locale = new RecipeTypeLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    recipeType.SetLocale(language, locale, _context.ActorId);

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    return await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
  }
}

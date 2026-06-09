using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientTypes.Commands;

internal record SaveIngredientTypeLocaleCommand(Guid Id, string Language, SaveIngredientTypeLocalePayload Payload) : ICommand<IngredientTypeModel?>;

internal class SaveIngredientTypeLocaleCommandHandler : ICommandHandler<SaveIngredientTypeLocaleCommand, IngredientTypeModel?>
{
  private readonly IContext _context;
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public SaveIngredientTypeLocaleCommandHandler(
    IContext context,
    IIngredientTypeQuerier ingredientTypeQuerier,
    IIngredientTypeRepository ingredientTypeRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientTypeQuerier = ingredientTypeQuerier;
    _ingredientTypeRepository = ingredientTypeRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientTypeModel?> HandleAsync(SaveIngredientTypeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveIngredientTypeLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientTypeId ingredientTypeId = new(_context.KitchenId, command.Id);
    IngredientType? ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken);
    if (ingredientType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientType, cancellationToken);

    IngredientTypeLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    ingredientType.SetLocale(language, locale, _context.ActorId);

    await _ingredientTypeRepository.SaveAsync(ingredientType, cancellationToken);

    return await _ingredientTypeQuerier.ReadAsync(ingredientType, cancellationToken);
  }
}

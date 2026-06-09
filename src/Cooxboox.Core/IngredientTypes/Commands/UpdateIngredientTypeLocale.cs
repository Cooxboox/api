using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientTypes.Commands;

internal record UpdateIngredientTypeLocaleCommand(Guid Id, string Language, UpdateIngredientTypeLocalePayload Payload) : ICommand<IngredientTypeModel?>;

internal class UpdateIngredientTypeLocaleCommandHandler : ICommandHandler<UpdateIngredientTypeLocaleCommand, IngredientTypeModel?>
{
  private readonly IContext _context;
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientTypeLocaleCommandHandler(
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

  public async Task<IngredientTypeModel?> HandleAsync(UpdateIngredientTypeLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateIngredientTypeLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientTypeId ingredientTypeId = new(_context.KitchenId, command.Id);
    IngredientType? ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken);
    if (ingredientType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientType, cancellationToken);

    IngredientTypeLocale locale = ingredientType.FindLocale(language);
    locale = new IngredientTypeLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    ingredientType.SetLocale(language, locale, _context.ActorId);

    await _ingredientTypeRepository.SaveAsync(ingredientType, cancellationToken);

    return await _ingredientTypeQuerier.ReadAsync(ingredientType, cancellationToken);
  }
}

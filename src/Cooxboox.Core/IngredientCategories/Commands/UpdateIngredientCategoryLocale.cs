using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientCategories.Commands;

internal record UpdateIngredientCategoryLocaleCommand(Guid Id, string Language, UpdateIngredientCategoryLocalePayload Payload) : ICommand<IngredientCategoryModel?>;

internal class UpdateIngredientCategoryLocaleCommandHandler : ICommandHandler<UpdateIngredientCategoryLocaleCommand, IngredientCategoryModel?>
{
  private readonly IContext _context;
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientCategoryLocaleCommandHandler(
    IContext context,
    IIngredientCategoryQuerier ingredientCategoryQuerier,
    IIngredientCategoryRepository ingredientCategoryRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientCategoryQuerier = ingredientCategoryQuerier;
    _ingredientCategoryRepository = ingredientCategoryRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientCategoryModel?> HandleAsync(UpdateIngredientCategoryLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateIngredientCategoryLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientCategoryId ingredientCategoryId = new(_context.KitchenId, command.Id);
    IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.LoadAsync(ingredientCategoryId, cancellationToken);
    if (ingredientCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientCategory, cancellationToken);

    IngredientCategoryLocale locale = ingredientCategory.FindLocale(language);
    locale = new IngredientCategoryLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    ingredientCategory.SetLocale(language, locale, _context.ActorId);

    await _ingredientCategoryRepository.SaveAsync(ingredientCategory, cancellationToken);

    return await _ingredientCategoryQuerier.ReadAsync(ingredientCategory, cancellationToken);
  }
}

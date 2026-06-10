using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientCategories.Commands;

internal record SaveIngredientCategoryLocaleCommand(Guid Id, string Language, SaveIngredientCategoryLocalePayload Payload) : ICommand<IngredientCategoryModel?>;

internal class SaveIngredientCategoryLocaleCommandHandler : ICommandHandler<SaveIngredientCategoryLocaleCommand, IngredientCategoryModel?>
{
  private readonly IContext _context;
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IPermissionService _permissionService;

  public SaveIngredientCategoryLocaleCommandHandler(
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

  public async Task<IngredientCategoryModel?> HandleAsync(SaveIngredientCategoryLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveIngredientCategoryLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientCategoryId ingredientCategoryId = new(_context.KitchenId, command.Id);
    IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.LoadAsync(ingredientCategoryId, cancellationToken);
    if (ingredientCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientCategory, cancellationToken);

    IngredientCategoryLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    ingredientCategory.SetLocale(language, locale, _context.ActorId);

    await _ingredientCategoryRepository.SaveAsync(ingredientCategory, cancellationToken);

    return await _ingredientCategoryQuerier.ReadAsync(ingredientCategory, cancellationToken);
  }
}

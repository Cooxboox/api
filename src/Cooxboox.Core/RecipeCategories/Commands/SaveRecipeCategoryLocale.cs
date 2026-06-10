using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeCategories.Commands;

internal record SaveRecipeCategoryLocaleCommand(Guid Id, string Language, SaveRecipeCategoryLocalePayload Payload) : ICommand<RecipeCategoryModel?>;

internal class SaveRecipeCategoryLocaleCommandHandler : ICommandHandler<SaveRecipeCategoryLocaleCommand, RecipeCategoryModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;

  public SaveRecipeCategoryLocaleCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeCategoryQuerier recipeCategoryQuerier,
    IRecipeCategoryRepository recipeCategoryRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeCategoryQuerier = recipeCategoryQuerier;
    _recipeCategoryRepository = recipeCategoryRepository;
  }

  public async Task<RecipeCategoryModel?> HandleAsync(SaveRecipeCategoryLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveRecipeCategoryLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeCategoryId recipeCategoryId = new(_context.KitchenId, command.Id);
    RecipeCategory? recipeCategory = await _recipeCategoryRepository.LoadAsync(recipeCategoryId, cancellationToken);
    if (recipeCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeCategory, cancellationToken);

    RecipeCategoryLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    recipeCategory.SetLocale(language, locale, _context.ActorId);

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    return await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
  }
}

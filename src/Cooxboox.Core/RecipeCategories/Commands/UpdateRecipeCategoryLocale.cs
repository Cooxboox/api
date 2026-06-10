using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeCategories.Commands;

internal record UpdateRecipeCategoryLocaleCommand(Guid Id, string Language, UpdateRecipeCategoryLocalePayload Payload) : ICommand<RecipeCategoryModel?>;

internal class UpdateRecipeCategoryLocaleCommandHandler : ICommandHandler<UpdateRecipeCategoryLocaleCommand, RecipeCategoryModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;

  public UpdateRecipeCategoryLocaleCommandHandler(
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

  public async Task<RecipeCategoryModel?> HandleAsync(UpdateRecipeCategoryLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateRecipeCategoryLocalePayload payload = command.Payload;
    payload.Validate();

    RecipeCategoryId recipeCategoryId = new(_context.KitchenId, command.Id);
    RecipeCategory? recipeCategory = await _recipeCategoryRepository.LoadAsync(recipeCategoryId, cancellationToken);
    if (recipeCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeCategory, cancellationToken);

    RecipeCategoryLocale locale = recipeCategory.FindLocale(language);
    locale = new RecipeCategoryLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    recipeCategory.SetLocale(language, locale, _context.ActorId);

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    return await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
  }
}

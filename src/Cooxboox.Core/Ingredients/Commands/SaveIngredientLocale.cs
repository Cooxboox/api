using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Ingredients.Commands;

internal record SaveIngredientLocaleCommand(Guid Id, string Language, SaveIngredientLocalePayload Payload) : ICommand<IngredientModel?>;

internal class SaveIngredientLocaleCommandHandler : ICommandHandler<SaveIngredientLocaleCommand, IngredientModel?>
{
  private readonly IContext _context;
  private readonly IIngredientQuerier _ingredientQuerier;
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IPermissionService _permissionService;

  public SaveIngredientLocaleCommandHandler(
    IContext context,
    IIngredientQuerier ingredientQuerier,
    IIngredientRepository ingredientRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientQuerier = ingredientQuerier;
    _ingredientRepository = ingredientRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientModel?> HandleAsync(SaveIngredientLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveIngredientLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientId ingredientId = new(_context.KitchenId, command.Id);
    Ingredient? ingredient = await _ingredientRepository.LoadAsync(ingredientId, cancellationToken);
    if (ingredient is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredient, cancellationToken);

    IngredientLocale locale = new(
      new Name(payload.Name),
      Slug.TryCreate(payload.Slug),
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    ingredient.SetLocale(language, locale, _context.ActorId);

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    return await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
  }
}

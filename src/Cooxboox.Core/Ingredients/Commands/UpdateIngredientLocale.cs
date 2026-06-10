using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Ingredients.Commands;

internal record UpdateIngredientLocaleCommand(Guid Id, string Language, UpdateIngredientLocalePayload Payload) : ICommand<IngredientModel?>;

internal class UpdateIngredientLocaleCommandHandler : ICommandHandler<UpdateIngredientLocaleCommand, IngredientModel?>
{
  private readonly IContext _context;
  private readonly IIngredientQuerier _ingredientQuerier;
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientLocaleCommandHandler(
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

  public async Task<IngredientModel?> HandleAsync(UpdateIngredientLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateIngredientLocalePayload payload = command.Payload;
    payload.Validate();

    IngredientId ingredientId = new(_context.KitchenId, command.Id);
    Ingredient? ingredient = await _ingredientRepository.LoadAsync(ingredientId, cancellationToken);
    if (ingredient is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredient, cancellationToken);

    IngredientLocale locale = ingredient.FindLocale(language);
    locale = new IngredientLocale(
      Name.TryCreate(payload.Name) ?? locale.Name,
      payload.Slug is null ? locale.Slug : Slug.TryCreate(payload.Slug.Value),
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    ingredient.SetLocale(language, locale, _context.ActorId);

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    return await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
  }
}

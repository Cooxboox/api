using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Commands;

internal record UpdateIngredientCategoryCommand(Guid Id, UpdateIngredientCategoryPayload Payload) : ICommand<IngredientCategoryModel?>;

internal class UpdateIngredientCategoryCommandHandler : ICommandHandler<UpdateIngredientCategoryCommand, IngredientCategoryModel?>
{
  private readonly IContext _context;
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientCategoryCommandHandler(
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

  public async Task<IngredientCategoryModel?> HandleAsync(UpdateIngredientCategoryCommand command, CancellationToken cancellationToken)
  {
    UpdateIngredientCategoryPayload payload = command.Payload;
    payload.Validate();

    IngredientCategoryId ingredientCategoryId = new(_context.KitchenId, command.Id);
    IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.LoadAsync(ingredientCategoryId, cancellationToken);
    if (ingredientCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredientCategory, cancellationToken);

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      ingredientCategory.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Notes is not null)
    {
      ingredientCategory.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    await _ingredientCategoryRepository.SaveAsync(ingredientCategory, cancellationToken);

    return await _ingredientCategoryQuerier.ReadAsync(ingredientCategory, cancellationToken);
  }
}

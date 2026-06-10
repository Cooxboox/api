using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Commands;

internal record CreateOrReplaceIngredientCategoryCommand(CreateOrReplaceIngredientCategoryPayload Payload, Guid? Id) : ICommand<CreateOrReplaceIngredientCategoryResult>;

internal class CreateOrReplaceIngredientCategoryCommandHandler : ICommandHandler<CreateOrReplaceIngredientCategoryCommand, CreateOrReplaceIngredientCategoryResult>
{
  private readonly IContext _context;
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceIngredientCategoryCommandHandler(
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

  public async Task<CreateOrReplaceIngredientCategoryResult> HandleAsync(CreateOrReplaceIngredientCategoryCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientCategoryPayload payload = command.Payload;
    payload.Validate();

    IngredientCategoryId ingredientCategoryId = IngredientCategoryId.NewId(_context.KitchenId);
    IngredientCategory? ingredientCategory = null;
    if (command.Id.HasValue)
    {
      ingredientCategoryId = new IngredientCategoryId(ingredientCategoryId.KitchenId, command.Id.Value);
      ingredientCategory = await _ingredientCategoryRepository.LoadAsync(ingredientCategoryId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);
    Notes? notes = Notes.TryCreate(payload.Notes);

    bool created = false;
    if (ingredientCategory is null)
    {
      await _permissionService.CheckAsync(Actions.CreateIngredientCategory, cancellationToken);

      ingredientCategory = new IngredientCategory(ingredientCategoryId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, ingredientCategory, cancellationToken);
    }

    ingredientCategory.Update(name, notes, actorId);

    await _ingredientCategoryRepository.SaveAsync(ingredientCategory, cancellationToken);

    IngredientCategoryModel model = await _ingredientCategoryQuerier.ReadAsync(ingredientCategory, cancellationToken);
    return new CreateOrReplaceIngredientCategoryResult(model, created);
  }
}

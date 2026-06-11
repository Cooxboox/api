using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Commands;

internal record CreateOrReplaceIngredientCommand(CreateOrReplaceIngredientPayload Payload, Guid? Id) : ICommand<CreateOrReplaceIngredientResult>;

internal class CreateOrReplaceIngredientCommandHandler : ICommandHandler<CreateOrReplaceIngredientCommand, CreateOrReplaceIngredientResult>
{
  private readonly IContext _context;
  private readonly IIngredientQuerier _ingredientQuerier;
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceIngredientCommandHandler(
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

  public async Task<CreateOrReplaceIngredientResult> HandleAsync(CreateOrReplaceIngredientCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientPayload payload = command.Payload;
    payload.Validate();

    IngredientId ingredientId = IngredientId.NewId(_context.KitchenId);
    Ingredient? ingredient = null;
    if (command.Id.HasValue)
    {
      ingredientId = new IngredientId(ingredientId.KitchenId, command.Id.Value);
      ingredient = await _ingredientRepository.LoadAsync(ingredientId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);

    bool created = false;
    if (ingredient is null)
    {
      await _permissionService.CheckAsync(Actions.CreateIngredient, cancellationToken);

      ingredient = new Ingredient(ingredientId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, ingredient, cancellationToken);

      ingredient.Rename(name, actorId);
    }

    ingredient.Annotate(Notes.TryCreate(payload.Notes), actorId);

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    IngredientModel model = await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
    return new CreateOrReplaceIngredientResult(model, created);
  }
}

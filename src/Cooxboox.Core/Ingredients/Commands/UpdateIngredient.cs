using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Commands;

internal record UpdateIngredientCommand(Guid Id, UpdateIngredientPayload Payload) : ICommand<IngredientModel?>;

internal class UpdateIngredientCommandHandler : ICommandHandler<UpdateIngredientCommand, IngredientModel?>
{
  private readonly IContext _context;
  private readonly IIngredientQuerier _ingredientQuerier;
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public UpdateIngredientCommandHandler(
    IContext context,
    IIngredientQuerier ingredientQuerier,
    IIngredientRepository ingredientRepository,
    IIngredientTypeRepository ingredientTypeRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientQuerier = ingredientQuerier;
    _ingredientRepository = ingredientRepository;
    _ingredientTypeRepository = ingredientTypeRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientModel?> HandleAsync(UpdateIngredientCommand command, CancellationToken cancellationToken)
  {
    UpdateIngredientPayload payload = command.Payload;
    payload.Validate();

    IngredientId ingredientId = new(_context.KitchenId, command.Id);
    Ingredient? ingredient = await _ingredientRepository.LoadAsync(ingredientId, cancellationToken);
    if (ingredient is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, ingredient, cancellationToken);

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      ingredient.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Notes is not null)
    {
      ingredient.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    if (payload.TypeId is not null)
    {
      IngredientType? ingredientType = null;
      if (payload.TypeId.Value.HasValue)
      {
        IngredientTypeId ingredientTypeId = new(ingredientId.KitchenId, payload.TypeId.Value.Value);
        ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken)
          ?? throw new EntityNotFoundException(new Entity(IngredientType.EntityKind, ingredientTypeId.EntityId, ingredientTypeId.KitchenId), nameof(payload.TypeId));
      }
      ingredient.SetType(ingredientType, actorId);
    }

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    return await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
  }
}

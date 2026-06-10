using Cooxboox.Core.Ingredients.Models;
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
  private readonly IPermissionService _permissionService;

  public UpdateIngredientCommandHandler(
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

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    return await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
  }
}

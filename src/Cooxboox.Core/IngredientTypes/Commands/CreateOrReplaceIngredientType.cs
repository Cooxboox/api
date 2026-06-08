using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Commands;

internal record CreateOrReplaceIngredientTypeCommand(CreateOrReplaceIngredientTypePayload Payload, Guid? Id) : ICommand<CreateOrReplaceIngredientTypeResult>;

internal class CreateOrReplaceIngredientTypeCommandHandler : ICommandHandler<CreateOrReplaceIngredientTypeCommand, CreateOrReplaceIngredientTypeResult>
{
  private readonly IContext _context;
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceIngredientTypeCommandHandler(IContext context, IIngredientTypeQuerier ingredientTypeQuerier, IIngredientTypeRepository ingredientTypeRepository, IPermissionService permissionService)
  {
    _context = context;
    _ingredientTypeQuerier = ingredientTypeQuerier;
    _ingredientTypeRepository = ingredientTypeRepository;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceIngredientTypeResult> HandleAsync(CreateOrReplaceIngredientTypeCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientTypePayload payload = command.Payload;
    payload.Validate();

    IngredientTypeId ingredientTypeId = IngredientTypeId.NewId(_context.KitchenId);
    IngredientType? ingredientType = null;
    if (command.Id.HasValue)
    {
      ingredientTypeId = new IngredientTypeId(ingredientTypeId.KitchenId, command.Id.Value);
      ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);
    Notes? notes = Notes.TryCreate(payload.Notes);

    bool created = false;
    if (ingredientType is null)
    {
      await _permissionService.CheckAsync(Actions.CreateIngredientType, cancellationToken);

      ingredientType = new IngredientType(ingredientTypeId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, ingredientType, cancellationToken);
    }

    ingredientType.Update(name, notes, actorId);

    await _ingredientTypeRepository.SaveAsync(ingredientType, cancellationToken);

    IngredientTypeModel model = await _ingredientTypeQuerier.ReadAsync(ingredientType, cancellationToken);
    return new CreateOrReplaceIngredientTypeResult(model, created);
  }
}

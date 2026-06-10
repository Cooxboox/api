using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeCategories.Commands;

internal record UpdateRecipeCategoryCommand(Guid Id, UpdateRecipeCategoryPayload Payload) : ICommand<RecipeCategoryModel?>;

internal class UpdateRecipeCategoryCommandHandler : ICommandHandler<UpdateRecipeCategoryCommand, RecipeCategoryModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;

  public UpdateRecipeCategoryCommandHandler(
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

  public async Task<RecipeCategoryModel?> HandleAsync(UpdateRecipeCategoryCommand command, CancellationToken cancellationToken)
  {
    UpdateRecipeCategoryPayload payload = command.Payload;
    payload.Validate();

    RecipeCategoryId recipeCategoryId = new(_context.KitchenId, command.Id);
    RecipeCategory? recipeCategory = await _recipeCategoryRepository.LoadAsync(recipeCategoryId, cancellationToken);
    if (recipeCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, recipeCategory, cancellationToken);

    Name name = Name.TryCreate(payload.Name) ?? recipeCategory.Name;
    Notes? notes = payload.Notes is null ? recipeCategory.Notes : Notes.TryCreate(payload.Notes.Value);

    recipeCategory.Update(name, notes, _context.ActorId);

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    return await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
  }
}

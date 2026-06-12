using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

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

    ActorId? actorId = _context.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      recipeCategory.Rename(new Name(payload.Name), actorId);
    }
    if (payload.Icon is not null)
    {
      recipeCategory.SetIcon(Icon.TryCreate(payload.Icon.Value), actorId);
    }
    if (payload.Notes is not null)
    {
      recipeCategory.Annotate(Notes.TryCreate(payload.Notes.Value), actorId);
    }

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    return await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
  }
}

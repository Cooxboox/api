using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Commands;

internal record CreateOrReplaceRecipeCategoryCommand(CreateOrReplaceRecipeCategoryPayload Payload, Guid? Id) : ICommand<CreateOrReplaceRecipeCategoryResult>;

internal class CreateOrReplaceRecipeCategoryCommandHandler : ICommandHandler<CreateOrReplaceRecipeCategoryCommand, CreateOrReplaceRecipeCategoryResult>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;

  public CreateOrReplaceRecipeCategoryCommandHandler(
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

  public async Task<CreateOrReplaceRecipeCategoryResult> HandleAsync(CreateOrReplaceRecipeCategoryCommand command, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeCategoryPayload payload = command.Payload;
    payload.Validate();

    RecipeCategoryId recipeCategoryId = RecipeCategoryId.NewId(_context.KitchenId);
    RecipeCategory? recipeCategory = null;
    if (command.Id.HasValue)
    {
      recipeCategoryId = new RecipeCategoryId(recipeCategoryId.KitchenId, command.Id.Value);
      recipeCategory = await _recipeCategoryRepository.LoadAsync(recipeCategoryId, cancellationToken);
    }

    ActorId? actorId = _context.ActorId;
    Name name = new(payload.Name);

    bool created = false;
    if (recipeCategory is null)
    {
      await _permissionService.CheckAsync(Actions.CreateRecipeCategory, cancellationToken);

      recipeCategory = new RecipeCategory(recipeCategoryId, name, actorId);
      created = true;
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, recipeCategory, cancellationToken);

      recipeCategory.Rename(name, actorId);
    }

    recipeCategory.Annotate(Notes.TryCreate(payload.Notes), actorId);
    recipeCategory.SetIcon(Icon.TryCreate(payload.Icon), actorId);

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    RecipeCategoryModel model = await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
    return new CreateOrReplaceRecipeCategoryResult(model, created);
  }
}

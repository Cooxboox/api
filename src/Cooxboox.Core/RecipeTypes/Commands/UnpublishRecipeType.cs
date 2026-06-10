using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record UnpublishRecipeTypeCommand : ICommand<RecipeTypeModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private UnpublishRecipeTypeCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static UnpublishRecipeTypeCommand All(Guid id) => new(id, language: null, all: true);
  public static UnpublishRecipeTypeCommand Invariant(Guid id) => new(id);
  public static UnpublishRecipeTypeCommand Locale(Guid id, string language) => new(id, language);
}

internal class UnpublishRecipeTypeCommandHandler : ICommandHandler<UnpublishRecipeTypeCommand, RecipeTypeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;

  public UnpublishRecipeTypeCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeTypeQuerier recipeTypeQuerier,
    IRecipeTypeRepository recipeTypeRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeTypeQuerier = recipeTypeQuerier;
    _recipeTypeRepository = recipeTypeRepository;
  }

  public async Task<RecipeTypeModel?> HandleAsync(UnpublishRecipeTypeCommand command, CancellationToken cancellationToken)
  {
    RecipeTypeId recipeTypeId = new(_context.KitchenId, command.Id);
    RecipeType? recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    if (recipeType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Unpublish, recipeType, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      recipeType.Unpublish(actorId);
    }
    else if (command.Language is null)
    {
      recipeType.UnpublishInvariant(actorId);
    }
    else
    {
      recipeType.UnpublishLocale(new Language(command.Language), actorId);
    }

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    return await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
  }
}

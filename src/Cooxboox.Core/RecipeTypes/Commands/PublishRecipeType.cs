using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeTypes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Commands;

internal record PublishRecipeTypeCommand : ICommand<RecipeTypeModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishRecipeTypeCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishRecipeTypeCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishRecipeTypeCommand Invariant(Guid id) => new(id);
  public static PublishRecipeTypeCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishRecipeTypeCommandHandler : ICommandHandler<PublishRecipeTypeCommand, RecipeTypeModel?>
{
  private readonly IContext _context;
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;
  private readonly IRecipeTypeRepository _recipeTypeRepository;
  private readonly IPermissionService _permissionService;

  public PublishRecipeTypeCommandHandler(
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

  public async Task<RecipeTypeModel?> HandleAsync(PublishRecipeTypeCommand command, CancellationToken cancellationToken)
  {
    RecipeTypeId recipeTypeId = new(_context.KitchenId, command.Id);
    RecipeType? recipeType = await _recipeTypeRepository.LoadAsync(recipeTypeId, cancellationToken);
    if (recipeType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, recipeType, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      recipeType.Publish(actorId);
    }
    else if (command.Language is null)
    {
      recipeType.PublishInvariant(actorId);
    }
    else
    {
      recipeType.PublishLocale(new Language(command.Language), actorId);
    }

    await _recipeTypeRepository.SaveAsync(recipeType, cancellationToken);

    return await _recipeTypeQuerier.ReadAsync(recipeType, cancellationToken);
  }
}

using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.RecipeCategories.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Commands;

internal record PublishRecipeCategoryCommand : ICommand<RecipeCategoryModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishRecipeCategoryCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishRecipeCategoryCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishRecipeCategoryCommand Invariant(Guid id) => new(id);
  public static PublishRecipeCategoryCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishRecipeCategoryCommandHandler : ICommandHandler<PublishRecipeCategoryCommand, RecipeCategoryModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;
  private readonly IRecipeCategoryRepository _recipeCategoryRepository;

  public PublishRecipeCategoryCommandHandler(
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

  public async Task<RecipeCategoryModel?> HandleAsync(PublishRecipeCategoryCommand command, CancellationToken cancellationToken)
  {
    RecipeCategoryId recipeCategoryId = new(_context.KitchenId, command.Id);
    RecipeCategory? recipeCategory = await _recipeCategoryRepository.LoadAsync(recipeCategoryId, cancellationToken);
    if (recipeCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, recipeCategory, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      recipeCategory.Publish(actorId);
    }
    else if (command.Language is null)
    {
      recipeCategory.PublishInvariant(actorId);
    }
    else
    {
      recipeCategory.PublishLocale(new Language(command.Language), actorId);
    }

    await _recipeCategoryRepository.SaveAsync(recipeCategory, cancellationToken);

    return await _recipeCategoryQuerier.ReadAsync(recipeCategory, cancellationToken);
  }
}

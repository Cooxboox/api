using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Commands;

internal record PublishIngredientCategoryCommand : ICommand<IngredientCategoryModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishIngredientCategoryCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishIngredientCategoryCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishIngredientCategoryCommand Invariant(Guid id) => new(id);
  public static PublishIngredientCategoryCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishIngredientCategoryCommandHandler : ICommandHandler<PublishIngredientCategoryCommand, IngredientCategoryModel?>
{
  private readonly IContext _context;
  private readonly IIngredientCategoryQuerier _ingredientCategoryQuerier;
  private readonly IIngredientCategoryRepository _ingredientCategoryRepository;
  private readonly IPermissionService _permissionService;

  public PublishIngredientCategoryCommandHandler(
    IContext context,
    IIngredientCategoryQuerier ingredientCategoryQuerier,
    IIngredientCategoryRepository ingredientCategoryRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientCategoryQuerier = ingredientCategoryQuerier;
    _ingredientCategoryRepository = ingredientCategoryRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientCategoryModel?> HandleAsync(PublishIngredientCategoryCommand command, CancellationToken cancellationToken)
  {
    IngredientCategoryId ingredientCategoryId = new(_context.KitchenId, command.Id);
    IngredientCategory? ingredientCategory = await _ingredientCategoryRepository.LoadAsync(ingredientCategoryId, cancellationToken);
    if (ingredientCategory is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, ingredientCategory, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      ingredientCategory.Publish(actorId);
    }
    else if (command.Language is null)
    {
      ingredientCategory.PublishInvariant(actorId);
    }
    else
    {
      ingredientCategory.PublishLocale(new Language(command.Language), actorId);
    }

    await _ingredientCategoryRepository.SaveAsync(ingredientCategory, cancellationToken);

    return await _ingredientCategoryQuerier.ReadAsync(ingredientCategory, cancellationToken);
  }
}

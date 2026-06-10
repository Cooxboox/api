using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Recipes.Models;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Commands;

internal record PublishRecipeCommand : ICommand<RecipeModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishRecipeCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishRecipeCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishRecipeCommand Invariant(Guid id) => new(id);
  public static PublishRecipeCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishRecipeCommandHandler : ICommandHandler<PublishRecipeCommand, RecipeModel?>
{
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;
  private readonly IRecipeQuerier _recipeQuerier;
  private readonly IRecipeRepository _recipeRepository;

  public PublishRecipeCommandHandler(
    IContext context,
    IPermissionService permissionService,
    IRecipeQuerier recipeQuerier,
    IRecipeRepository recipeRepository)
  {
    _context = context;
    _permissionService = permissionService;
    _recipeQuerier = recipeQuerier;
    _recipeRepository = recipeRepository;
  }

  public async Task<RecipeModel?> HandleAsync(PublishRecipeCommand command, CancellationToken cancellationToken)
  {
    RecipeId recipeId = new(_context.KitchenId, command.Id);
    Recipe? recipe = await _recipeRepository.LoadAsync(recipeId, cancellationToken);
    if (recipe is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, recipe, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      recipe.Publish(actorId);
    }
    else if (command.Language is null)
    {
      recipe.PublishInvariant(actorId);
    }
    else
    {
      recipe.PublishLocale(new Language(command.Language), actorId);
    }

    await _recipeRepository.SaveAsync(recipe, cancellationToken);

    return await _recipeQuerier.ReadAsync(recipe, cancellationToken);
  }
}

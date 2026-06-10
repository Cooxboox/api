using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Commands;

internal record PublishIngredientCommand : ICommand<IngredientModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishIngredientCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishIngredientCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishIngredientCommand Invariant(Guid id) => new(id);
  public static PublishIngredientCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishIngredientCommandHandler : ICommandHandler<PublishIngredientCommand, IngredientModel?>
{
  private readonly IContext _context;
  private readonly IIngredientQuerier _ingredientQuerier;
  private readonly IIngredientRepository _ingredientRepository;
  private readonly IPermissionService _permissionService;

  public PublishIngredientCommandHandler(
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

  public async Task<IngredientModel?> HandleAsync(PublishIngredientCommand command, CancellationToken cancellationToken)
  {
    IngredientId ingredientId = new(_context.KitchenId, command.Id);
    Ingredient? ingredient = await _ingredientRepository.LoadAsync(ingredientId, cancellationToken);
    if (ingredient is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, ingredient, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      ingredient.Publish(actorId);
    }
    else if (command.Language is null)
    {
      ingredient.PublishInvariant(actorId);
    }
    else
    {
      ingredient.PublishLocale(new Language(command.Language), actorId);
    }

    await _ingredientRepository.SaveAsync(ingredient, cancellationToken);

    return await _ingredientQuerier.ReadAsync(ingredient, cancellationToken);
  }
}

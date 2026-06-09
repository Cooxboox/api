using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Commands;

internal record PublishIngredientTypeCommand : ICommand<IngredientTypeModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishIngredientTypeCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishIngredientTypeCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishIngredientTypeCommand Invariant(Guid id) => new(id);
  public static PublishIngredientTypeCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishIngredientTypeHandler : ICommandHandler<PublishIngredientTypeCommand, IngredientTypeModel?>
{
  private readonly IContext _context;
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;
  private readonly IIngredientTypeRepository _ingredientTypeRepository;
  private readonly IPermissionService _permissionService;

  public PublishIngredientTypeHandler(
    IContext context,
    IIngredientTypeQuerier ingredientTypeQuerier,
    IIngredientTypeRepository ingredientTypeRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _ingredientTypeQuerier = ingredientTypeQuerier;
    _ingredientTypeRepository = ingredientTypeRepository;
    _permissionService = permissionService;
  }

  public async Task<IngredientTypeModel?> HandleAsync(PublishIngredientTypeCommand command, CancellationToken cancellationToken)
  {
    IngredientTypeId ingredientTypeId = new(_context.KitchenId, command.Id);
    IngredientType? ingredientType = await _ingredientTypeRepository.LoadAsync(ingredientTypeId, cancellationToken);
    if (ingredientType is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, ingredientType, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      ingredientType.Publish(actorId);
    }
    else if (command.Language is null)
    {
      ingredientType.PublishInvariant(actorId);
    }
    else
    {
      ingredientType.PublishLocale(new Language(command.Language), actorId);
    }

    await _ingredientTypeRepository.SaveAsync(ingredientType, cancellationToken);

    return await _ingredientTypeQuerier.ReadAsync(ingredientType, cancellationToken);
  }
}

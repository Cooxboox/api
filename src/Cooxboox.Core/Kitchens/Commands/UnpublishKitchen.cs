using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UnpublishKitchenCommand : ICommand<KitchenModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private UnpublishKitchenCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static UnpublishKitchenCommand All(Guid id) => new(id, language: null, all: true);
  public static UnpublishKitchenCommand Invariant(Guid id) => new(id);
  public static UnpublishKitchenCommand Locale(Guid id, string language) => new(id, language);
}

internal class UnpublishKitchenCommandHandler : ICommandHandler<UnpublishKitchenCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public UnpublishKitchenCommandHandler(
    IContext context,
    IKitchenQuerier kitchenQuerier,
    IKitchenRepository kitchenRepository,
    IPermissionService permissionService)
  {
    _context = context;
    _kitchenQuerier = kitchenQuerier;
    _kitchenRepository = kitchenRepository;
    _permissionService = permissionService;
  }

  public async Task<KitchenModel?> HandleAsync(UnpublishKitchenCommand command, CancellationToken cancellationToken)
  {
    KitchenId kitchenId = new(command.Id);
    Kitchen? kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Unpublish, kitchen, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      kitchen.Unpublish(actorId);
    }
    else if (command.Language is null)
    {
      kitchen.UnpublishInvariant(actorId);
    }
    else
    {
      kitchen.UnpublishLocale(new Language(command.Language), actorId);
    }

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

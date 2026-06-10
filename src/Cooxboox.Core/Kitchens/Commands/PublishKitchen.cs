using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Logitar.CQRS;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Commands;

internal record PublishKitchenCommand : ICommand<KitchenModel?>
{
  public Guid Id { get; }
  public string? Language { get; }
  public bool IsAll { get; }

  private PublishKitchenCommand(Guid id, string? language = null, bool all = false)
  {
    if (language is not null && string.IsNullOrWhiteSpace(language))
    {
      throw new ArgumentOutOfRangeException(nameof(language));
    }

    Id = id;
    Language = language;
    IsAll = all;
  }

  public static PublishKitchenCommand All(Guid id) => new(id, language: null, all: true);
  public static PublishKitchenCommand Invariant(Guid id) => new(id);
  public static PublishKitchenCommand Locale(Guid id, string language) => new(id, language);
}

internal class PublishKitchenCommandHandler : ICommandHandler<PublishKitchenCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public PublishKitchenCommandHandler(
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

  public async Task<KitchenModel?> HandleAsync(PublishKitchenCommand command, CancellationToken cancellationToken)
  {
    KitchenId kitchenId = new(command.Id);
    Kitchen? kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Publish, kitchen, cancellationToken);

    ActorId? actorId = _context.ActorId;
    if (command.IsAll)
    {
      kitchen.Publish(actorId);
    }
    else if (command.Language is null)
    {
      kitchen.PublishInvariant(actorId);
    }
    else
    {
      kitchen.PublishLocale(new Language(command.Language), actorId);
    }

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record SaveKitchenLocaleCommand(Guid Id, string Language, SaveKitchenLocalePayload Payload) : ICommand<KitchenModel?>;

internal class SaveKitchenLocaleCommandHandler : ICommandHandler<SaveKitchenLocaleCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public SaveKitchenLocaleCommandHandler(
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

  public async Task<KitchenModel?> HandleAsync(SaveKitchenLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    SaveKitchenLocalePayload payload = command.Payload;
    payload.Validate();

    KitchenId kitchenId = new(command.Id);
    Kitchen? kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

    KitchenLocale locale = new(
      MetaDescription.TryCreate(payload.MetaDescription),
      HtmlContent.TryCreate(payload.HtmlContent),
      Notes.TryCreate(payload.Notes));
    kitchen.SetLocale(language, locale, _context.ActorId);

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

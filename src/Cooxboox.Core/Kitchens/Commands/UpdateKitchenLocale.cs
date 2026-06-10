using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Cooxboox.Core.Seo;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UpdateKitchenLocaleCommand(Guid Id, string Language, UpdateKitchenLocalePayload Payload) : ICommand<KitchenModel?>;

internal class UpdateKitchenLocaleCommandHandler : ICommandHandler<UpdateKitchenLocaleCommand, KitchenModel?>
{
  private readonly IContext _context;
  private readonly IKitchenQuerier _kitchenQuerier;
  private readonly IKitchenRepository _kitchenRepository;
  private readonly IPermissionService _permissionService;

  public UpdateKitchenLocaleCommandHandler(
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

  public async Task<KitchenModel?> HandleAsync(UpdateKitchenLocaleCommand command, CancellationToken cancellationToken)
  {
    Language language = new(command.Language);

    UpdateKitchenLocalePayload payload = command.Payload;
    payload.Validate();

    KitchenId kitchenId = new(command.Id);
    Kitchen? kitchen = await _kitchenRepository.LoadAsync(kitchenId, cancellationToken);
    if (kitchen is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, kitchen, cancellationToken);

    KitchenLocale locale = kitchen.FindLocale(language);
    locale = new KitchenLocale(
      payload.MetaDescription is null ? locale.MetaDescription : MetaDescription.TryCreate(payload.MetaDescription.Value),
      payload.HtmlContent is null ? locale.HtmlContent : HtmlContent.TryCreate(payload.HtmlContent.Value),
      payload.Notes is null ? locale.Notes : Notes.TryCreate(payload.Notes.Value));
    kitchen.SetLocale(language, locale, _context.ActorId);

    await _kitchenRepository.SaveAsync(kitchen, cancellationToken);

    return await _kitchenQuerier.ReadAsync(kitchen, cancellationToken);
  }
}

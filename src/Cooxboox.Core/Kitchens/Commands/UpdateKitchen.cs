using Cooxboox.Core.Contents;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Contents;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record UpdateKitchenCommand(Guid Id, UpdateKitchenPayload Payload) : ICommand<KitchenModel?>;

internal class UpdateKitchenCommandHandler : ICommandHandler<UpdateKitchenCommand, KitchenModel?>
{
  private readonly IContentGateway _contentGateway;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public UpdateKitchenCommandHandler(IContentGateway contentGateway, IContext context, IPermissionService permissionService)
  {
    _contentGateway = contentGateway;
    _context = context;
    _permissionService = permissionService;
  }

  public async Task<KitchenModel?> HandleAsync(UpdateKitchenCommand command, CancellationToken cancellationToken)
  {
    UpdateKitchenPayload payload = command.Payload;
    payload.Validate();

    Content? content = await _contentGateway.FindAsync(KitchenDefinition.ContentTypeId, command.Id.ToString(), cancellationToken);
    if (content is null)
    {
      return null;
    }
    await _permissionService.CheckAsync(Actions.Update, content, cancellationToken);

    UpdateContentLocaleOptions options = new();

    if (!string.IsNullOrWhiteSpace(payload.Name))
    {
      options.DisplayName = new Optional<string>(payload.Name);
    }

    content = await _contentGateway.UpdateLocaleAsync(content.Id, options, cancellationToken);

    return Mapper.ToKitchen(content);
  }
}

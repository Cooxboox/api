using Cooxboox.Core.Contents;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Permissions;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Logitar.CQRS;

namespace Cooxboox.Core.Kitchens.Commands;

internal record CreateOrReplaceKitchenCommand(CreateOrReplaceKitchenPayload Payload, Guid? Id, string? Language) : ICommand<CreateOrReplaceKitchenResult>;

internal class CreateOrReplaceKitchenCommandHandler : ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>
{
  private readonly IContentGateway _contentGateway;
  private readonly IContext _context;
  private readonly IPermissionService _permissionService;

  public CreateOrReplaceKitchenCommandHandler(IContentGateway contentGateway, IContext context, IPermissionService permissionService)
  {
    _contentGateway = contentGateway;
    _context = context;
    _permissionService = permissionService;
  }

  public async Task<CreateOrReplaceKitchenResult> HandleAsync(CreateOrReplaceKitchenCommand command, CancellationToken cancellationToken)
  {
    Language language = string.IsNullOrWhiteSpace(command.Language) ? Language.Default : new(command.Language);

    CreateOrReplaceKitchenPayload payload = command.Payload;
    payload.Validate();

    Content? content = null;
    string uniqueName = (command.Id ?? Guid.NewGuid()).ToString();
    if (command.Id.HasValue)
    {
      content = await _contentGateway.FindAsync(KitchenDefinition.ContentTypeId, uniqueName, cancellationToken);
    }

    bool created = false;
    if (content is null)
    {
      await _permissionService.CheckAsync(Actions.CreateKitchen, cancellationToken);

      CreateContentOptions options = new()
      {
        Language = language,
        DisplayName = payload.Name
      };
      options.FieldValues[KitchenDefinition.Owner] = _context.UserId.EntityId.ToString();
      options.FieldValues[KitchenDefinition.Confidentiality] = $@"[""{Confidentiality.Private}""]";
      content = await _contentGateway.CreateAsync(KitchenDefinition.ContentTypeId, uniqueName, options, cancellationToken);
    }
    else
    {
      await _permissionService.CheckAsync(Actions.Update, content, cancellationToken);

      SaveContentLocaleOptions options = new()
      {
        DisplayName = payload.Name
      };
      foreach (FieldValue field in content.Invariant.FieldValues)
      {
        options.FieldValues[field.Id] = field.Value;
      }
      content = await _contentGateway.SaveLocaleAsync(content.Id, uniqueName, options, cancellationToken);
    }

    KitchenModel model = Mapper.ToKitchen(content);
    return new CreateOrReplaceKitchenResult(model, created);
  }
}

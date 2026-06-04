using Cooxboox.Core;
using Cooxboox.Core.Contents;
using Cooxboox.Infrastructure.Identity;
using Krakenar.Client;
using Krakenar.Client.Contents;
using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;

namespace Cooxboox.Infrastructure.Contents;

internal class ContentGateway : IContentGateway
{
  private readonly IContentClient _contentClient;
  private readonly IContext _context;

  public ContentGateway(IContentClient contentClient, IContext context)
  {
    _contentClient = contentClient;
    _context = context;
  }

  public async Task<Content> CreateAsync(Guid contentTypeId, string uniqueName, CreateContentOptions? options, CancellationToken cancellationToken)
  {
    options ??= new();

    CreateContentPayload payload = new()
    {
      Id = options.Id,
      ContentType = contentTypeId.ToString(),
      Language = options.Language?.Code,
      UniqueName = uniqueName,
      DisplayName = options.DisplayName,
      Description = options.Description
    };

    foreach (KeyValuePair<Guid, string> field in options.FieldValues)
    {
      payload.FieldValues.Add(new FieldValuePayload(field.Key.ToString(), field.Value));
    }

    RequestContext context = new RequestContextBuilder(cancellationToken).WithUserId(_context.UserId).Build();
    return await _contentClient.CreateAsync(payload, context);
  }

  public async Task<Content?> FindAsync(Guid contentTypeId, string uniqueName, CancellationToken cancellationToken)
  {
    SearchContentLocalesPayload payload = new()
    {
      ContentTypeId = contentTypeId
    };
    payload.Search.Terms.Add(new SearchTerm(uniqueName));

    SearchResults<ContentLocale> results = await _contentClient.SearchLocalesAsync(payload, cancellationToken);
    return results.Items.SingleOrDefault(locale => locale.UniqueName.Equals(uniqueName, StringComparison.InvariantCultureIgnoreCase))?.Content;
  }

  public async Task<Content> SaveLocaleAsync(Guid id, string uniqueName, SaveContentLocaleOptions? options, CancellationToken cancellationToken)
  {
    options ??= new();

    SaveContentLocalePayload payload = new()
    {
      UniqueName = uniqueName,
      DisplayName = options.DisplayName,
      Description = options.Description
    };

    foreach (KeyValuePair<Guid, string> field in options.FieldValues)
    {
      payload.FieldValues.Add(new FieldValuePayload(field.Key.ToString(), field.Value));
    }

    RequestContext context = new RequestContextBuilder(cancellationToken).WithUserId(_context.UserId).Build();
    return await _contentClient.SaveLocaleAsync(id, payload, options.Language?.Code, context)
      ?? throw new InvalidOperationException($"The updated content 'Id={id}' was not found.");
  }

  public async Task<Content> UpdateLocaleAsync(Guid id, UpdateContentLocaleOptions? options, CancellationToken cancellationToken)
  {
    options ??= new();

    UpdateContentLocalePayload payload = new()
    {
      UniqueName = options.UniqueName,
      DisplayName = options.DisplayName is null ? null : new Change<string>(options.DisplayName.Value),
      Description = options.Description is null ? null : new Change<string>(options.Description.Value)
    };

    foreach (KeyValuePair<Guid, string> field in options.FieldValues)
    {
      payload.FieldValues.Add(new FieldValuePayload(field.Key.ToString(), field.Value));
    }

    RequestContext context = new RequestContextBuilder(cancellationToken).WithUserId(_context.UserId).Build();
    return await _contentClient.UpdateLocaleAsync(id, payload, options.Language?.Code, context)
      ?? throw new InvalidOperationException($"The updated content 'Id={id}' was not found.");
  }
}

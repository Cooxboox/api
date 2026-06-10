using Cooxboox.Core.Kitchens.Commands;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Core.Kitchens.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenService
{
  Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<KitchenModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<KitchenModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<KitchenModel?> SaveLocaleAsync(Guid id, string language, SaveKitchenLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<KitchenModel>> SearchAsync(SearchKitchensPayload payload, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken = default);
  Task<KitchenModel?> UpdateLocaleAsync(Guid id, string language, UpdateKitchenLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class KitchenService : IKitchenService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IKitchenService, KitchenService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceKitchenCommand, CreateOrReplaceKitchenResult>, CreateOrReplaceKitchenCommandHandler>();
    services.AddTransient<ICommandHandler<PublishKitchenCommand, KitchenModel?>, PublishKitchenCommandHandler>();
    services.AddTransient<ICommandHandler<SaveKitchenLocaleCommand, KitchenModel?>, SaveKitchenLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishKitchenCommand, KitchenModel?>, UnpublishKitchenCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateKitchenCommand, KitchenModel?>, UpdateKitchenCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateKitchenLocaleCommand, KitchenModel?>, UpdateKitchenLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadKitchenQuery, KitchenModel?>, ReadKitchenQueryHandler>();
    services.AddTransient<IQueryHandler<SearchKitchensQuery, SearchResults<KitchenModel>>, SearchKitchensQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public KitchenService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceKitchenResult> CreateOrReplaceAsync(CreateOrReplaceKitchenPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishKitchenCommand command = PublishKitchenCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishKitchenCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishKitchenCommand.Invariant(id)
      : PublishKitchenCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadKitchenQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<KitchenModel?> SaveLocaleAsync(Guid id, string language, SaveKitchenLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveKitchenLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<KitchenModel>> SearchAsync(SearchKitchensPayload payload, CancellationToken cancellationToken)
  {
    SearchKitchensQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<KitchenModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishKitchenCommand command = UnpublishKitchenCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishKitchenCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishKitchenCommand.Invariant(id)
      : UnpublishKitchenCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> UpdateAsync(Guid id, UpdateKitchenPayload payload, CancellationToken cancellationToken)
  {
    UpdateKitchenCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<KitchenModel?> UpdateLocaleAsync(Guid id, string language, UpdateKitchenLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateKitchenLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

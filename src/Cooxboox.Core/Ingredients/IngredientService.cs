using Cooxboox.Core.Ingredients.Commands;
using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Core.Ingredients.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Ingredients;

public interface IIngredientService
{
  Task<CreateOrReplaceIngredientResult> CreateOrReplaceAsync(CreateOrReplaceIngredientPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<IngredientModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<IngredientModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<IngredientModel>> SearchAsync(SearchIngredientsPayload payload, CancellationToken cancellationToken = default);
  Task<IngredientModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<IngredientModel?> UpdateAsync(Guid id, UpdateIngredientPayload payload, CancellationToken cancellationToken = default);
  Task<IngredientModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class IngredientService : IIngredientService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IIngredientService, IngredientService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceIngredientCommand, CreateOrReplaceIngredientResult>, CreateOrReplaceIngredientCommandHandler>();
    services.AddTransient<ICommandHandler<SaveIngredientLocaleCommand, IngredientModel?>, SaveIngredientLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<PublishIngredientCommand, IngredientModel?>, PublishIngredientCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishIngredientCommand, IngredientModel?>, UnpublishIngredientCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientCommand, IngredientModel?>, UpdateIngredientCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientLocaleCommand, IngredientModel?>, UpdateIngredientLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadIngredientQuery, IngredientModel?>, ReadIngredientQueryHandler>();
    services.AddTransient<IQueryHandler<SearchIngredientsQuery, SearchResults<IngredientModel>>, SearchIngredientsQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public IngredientService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceIngredientResult> CreateOrReplaceAsync(CreateOrReplaceIngredientPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishIngredientCommand command = PublishIngredientCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishIngredientCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishIngredientCommand.Invariant(id)
      : PublishIngredientCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadIngredientQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveIngredientLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<IngredientModel>> SearchAsync(SearchIngredientsPayload payload, CancellationToken cancellationToken)
  {
    SearchIngredientsQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishIngredientCommand command = UnpublishIngredientCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishIngredientCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishIngredientCommand.Invariant(id)
      : UnpublishIngredientCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> UpdateAsync(Guid id, UpdateIngredientPayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

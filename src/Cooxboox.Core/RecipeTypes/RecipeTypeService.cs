using Cooxboox.Core.RecipeTypes.Commands;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Core.RecipeTypes.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.RecipeTypes;

public interface IRecipeTypeService
{
  Task<CreateOrReplaceRecipeTypeResult> CreateOrReplaceAsync(CreateOrReplaceRecipeTypePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeTypeLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<RecipeTypeModel>> SearchAsync(SearchRecipeTypesPayload payload, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> UpdateAsync(Guid id, UpdateRecipeTypePayload payload, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeTypeLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class RecipeTypeService : IRecipeTypeService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IRecipeTypeService, RecipeTypeService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceRecipeTypeCommand, CreateOrReplaceRecipeTypeResult>, CreateOrReplaceRecipeTypeCommandHandler>();
    services.AddTransient<ICommandHandler<SaveRecipeTypeLocaleCommand, RecipeTypeModel?>, SaveRecipeTypeLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<PublishRecipeTypeCommand, RecipeTypeModel?>, PublishRecipeTypeCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishRecipeTypeCommand, RecipeTypeModel?>, UnpublishRecipeTypeCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeTypeCommand, RecipeTypeModel?>, UpdateRecipeTypeCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeTypeLocaleCommand, RecipeTypeModel?>, UpdateRecipeTypeLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadRecipeTypeQuery, RecipeTypeModel?>, ReadRecipeTypeQueryHandler>();
    services.AddTransient<IQueryHandler<SearchRecipeTypesQuery, SearchResults<RecipeTypeModel>>, SearchRecipeTypesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public RecipeTypeService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceRecipeTypeResult> CreateOrReplaceAsync(CreateOrReplaceRecipeTypePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeTypeCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishRecipeTypeCommand command = PublishRecipeTypeCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishRecipeTypeCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishRecipeTypeCommand.Invariant(id)
      : PublishRecipeTypeCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRecipeTypeQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeTypeModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveRecipeTypeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<RecipeTypeModel>> SearchAsync(SearchRecipeTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchRecipeTypesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeTypeModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishRecipeTypeCommand command = UnpublishRecipeTypeCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishRecipeTypeCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishRecipeTypeCommand.Invariant(id)
      : UnpublishRecipeTypeCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> UpdateAsync(Guid id, UpdateRecipeTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeTypeCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeTypeModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeTypeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

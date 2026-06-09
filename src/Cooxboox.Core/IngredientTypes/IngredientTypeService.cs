using Cooxboox.Core.IngredientTypes.Commands;
using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Core.IngredientTypes.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.IngredientTypes;

public interface IIngredientTypeService
{
  Task<CreateOrReplaceIngredientTypeResult> CreateOrReplaceAsync(CreateOrReplaceIngredientTypePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientTypeLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<IngredientTypeModel>> SearchAsync(SearchIngredientTypesPayload payload, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> UpdateAsync(Guid id, UpdateIngredientTypePayload payload, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientTypeLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class IngredientTypeService : IIngredientTypeService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IIngredientTypeService, IngredientTypeService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceIngredientTypeCommand, CreateOrReplaceIngredientTypeResult>, CreateOrReplaceIngredientTypeCommandHandler>();
    services.AddTransient<ICommandHandler<SaveIngredientTypeLocaleCommand, IngredientTypeModel?>, SaveIngredientTypeLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientTypeCommand, IngredientTypeModel?>, UpdateIngredientTypeCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientTypeLocaleCommand, IngredientTypeModel?>, UpdateIngredientTypeLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadIngredientTypeQuery, IngredientTypeModel?>, ReadIngredientTypeQueryHandler>();
    services.AddTransient<IQueryHandler<SearchIngredientTypesQuery, SearchResults<IngredientTypeModel>>, SearchIngredientTypesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public IngredientTypeService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceIngredientTypeResult> CreateOrReplaceAsync(CreateOrReplaceIngredientTypePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientTypeCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadIngredientTypeQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientTypeModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveIngredientTypeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<IngredientTypeModel>> SearchAsync(SearchIngredientTypesPayload payload, CancellationToken cancellationToken)
  {
    SearchIngredientTypesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientTypeModel?> UpdateAsync(Guid id, UpdateIngredientTypePayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientTypeCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientTypeModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientTypeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

using Cooxboox.Core.IngredientCategories.Commands;
using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Core.IngredientCategories.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.IngredientCategories;

public interface IIngredientCategoryService
{
  Task<CreateOrReplaceIngredientCategoryResult> CreateOrReplaceAsync(CreateOrReplaceIngredientCategoryPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientCategoryLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<IngredientCategoryModel>> SearchAsync(SearchIngredientCategoriesPayload payload, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> UpdateAsync(Guid id, UpdateIngredientCategoryPayload payload, CancellationToken cancellationToken = default);
  Task<IngredientCategoryModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientCategoryLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class IngredientCategoryService : IIngredientCategoryService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IIngredientCategoryService, IngredientCategoryService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceIngredientCategoryCommand, CreateOrReplaceIngredientCategoryResult>, CreateOrReplaceIngredientCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<SaveIngredientCategoryLocaleCommand, IngredientCategoryModel?>, SaveIngredientCategoryLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<PublishIngredientCategoryCommand, IngredientCategoryModel?>, PublishIngredientCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishIngredientCategoryCommand, IngredientCategoryModel?>, UnpublishIngredientCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientCategoryCommand, IngredientCategoryModel?>, UpdateIngredientCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateIngredientCategoryLocaleCommand, IngredientCategoryModel?>, UpdateIngredientCategoryLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadIngredientCategoryQuery, IngredientCategoryModel?>, ReadIngredientCategoryQueryHandler>();
    services.AddTransient<IQueryHandler<SearchIngredientCategoriesQuery, SearchResults<IngredientCategoryModel>>, SearchIngredientCategoriesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public IngredientCategoryService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceIngredientCategoryResult> CreateOrReplaceAsync(CreateOrReplaceIngredientCategoryPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientCategoryCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishIngredientCategoryCommand command = PublishIngredientCategoryCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishIngredientCategoryCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishIngredientCategoryCommand.Invariant(id)
      : PublishIngredientCategoryCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadIngredientCategoryQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> SaveLocaleAsync(Guid id, string language, SaveIngredientCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveIngredientCategoryLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<IngredientCategoryModel>> SearchAsync(SearchIngredientCategoriesPayload payload, CancellationToken cancellationToken)
  {
    SearchIngredientCategoriesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishIngredientCategoryCommand command = UnpublishIngredientCategoryCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishIngredientCategoryCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishIngredientCategoryCommand.Invariant(id)
      : UnpublishIngredientCategoryCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> UpdateAsync(Guid id, UpdateIngredientCategoryPayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientCategoryCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<IngredientCategoryModel?> UpdateLocaleAsync(Guid id, string language, UpdateIngredientCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateIngredientCategoryLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

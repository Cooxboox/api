using Cooxboox.Core.RecipeCategories.Commands;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Core.RecipeCategories.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.RecipeCategories;

public interface IRecipeCategoryService
{
  Task<CreateOrReplaceRecipeCategoryResult> CreateOrReplaceAsync(CreateOrReplaceRecipeCategoryPayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeCategoryLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<RecipeCategoryModel>> SearchAsync(SearchRecipeCategoriesPayload payload, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> UpdateAsync(Guid id, UpdateRecipeCategoryPayload payload, CancellationToken cancellationToken = default);
  Task<RecipeCategoryModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeCategoryLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class RecipeCategoryService : IRecipeCategoryService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IRecipeCategoryService, RecipeCategoryService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceRecipeCategoryCommand, CreateOrReplaceRecipeCategoryResult>, CreateOrReplaceRecipeCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<SaveRecipeCategoryLocaleCommand, RecipeCategoryModel?>, SaveRecipeCategoryLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<PublishRecipeCategoryCommand, RecipeCategoryModel?>, PublishRecipeCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishRecipeCategoryCommand, RecipeCategoryModel?>, UnpublishRecipeCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeCategoryCommand, RecipeCategoryModel?>, UpdateRecipeCategoryCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeCategoryLocaleCommand, RecipeCategoryModel?>, UpdateRecipeCategoryLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadRecipeCategoryQuery, RecipeCategoryModel?>, ReadRecipeCategoryQueryHandler>();
    services.AddTransient<IQueryHandler<SearchRecipeCategoriesQuery, SearchResults<RecipeCategoryModel>>, SearchRecipeCategoriesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public RecipeCategoryService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceRecipeCategoryResult> CreateOrReplaceAsync(CreateOrReplaceRecipeCategoryPayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeCategoryCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishRecipeCategoryCommand command = PublishRecipeCategoryCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishRecipeCategoryCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishRecipeCategoryCommand.Invariant(id)
      : PublishRecipeCategoryCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRecipeCategoryQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveRecipeCategoryLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<RecipeCategoryModel>> SearchAsync(SearchRecipeCategoriesPayload payload, CancellationToken cancellationToken)
  {
    SearchRecipeCategoriesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishRecipeCategoryCommand command = UnpublishRecipeCategoryCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishRecipeCategoryCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishRecipeCategoryCommand.Invariant(id)
      : UnpublishRecipeCategoryCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> UpdateAsync(Guid id, UpdateRecipeCategoryPayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeCategoryCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeCategoryModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeCategoryLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

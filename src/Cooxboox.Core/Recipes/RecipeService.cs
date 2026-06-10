using Cooxboox.Core.Recipes.Commands;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Core.Recipes.Queries;
using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Core.Recipes;

public interface IRecipeService
{
  Task<CreateOrReplaceRecipeResult> CreateOrReplaceAsync(CreateOrReplaceRecipePayload payload, Guid? id = null, CancellationToken cancellationToken = default);
  Task<RecipeModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeModel?> PublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeLocalePayload payload, CancellationToken cancellationToken = default);
  Task<SearchResults<RecipeModel>> SearchAsync(SearchRecipesPayload payload, CancellationToken cancellationToken = default);
  Task<RecipeModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken = default);
  Task<RecipeModel?> UnpublishAsync(Guid id, string? language = null, CancellationToken cancellationToken = default);
  Task<RecipeModel?> UpdateAsync(Guid id, UpdateRecipePayload payload, CancellationToken cancellationToken = default);
  Task<RecipeModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeLocalePayload payload, CancellationToken cancellationToken = default);
}

internal class RecipeService : IRecipeService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IRecipeService, RecipeService>();
    services.AddTransient<ICommandHandler<CreateOrReplaceRecipeCommand, CreateOrReplaceRecipeResult>, CreateOrReplaceRecipeCommandHandler>();
    services.AddTransient<ICommandHandler<SaveRecipeLocaleCommand, RecipeModel?>, SaveRecipeLocaleCommandHandler>();
    services.AddTransient<ICommandHandler<PublishRecipeCommand, RecipeModel?>, PublishRecipeCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishRecipeCommand, RecipeModel?>, UnpublishRecipeCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeCommand, RecipeModel?>, UpdateRecipeCommandHandler>();
    services.AddTransient<ICommandHandler<UpdateRecipeLocaleCommand, RecipeModel?>, UpdateRecipeLocaleCommandHandler>();
    services.AddTransient<IQueryHandler<ReadRecipeQuery, RecipeModel?>, ReadRecipeQueryHandler>();
    services.AddTransient<IQueryHandler<SearchRecipesQuery, SearchResults<RecipeModel>>, SearchRecipesQueryHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IQueryBus _queryBus;

  public RecipeService(ICommandBus commandBus, IQueryBus queryBus)
  {
    _commandBus = commandBus;
    _queryBus = queryBus;
  }

  public async Task<CreateOrReplaceRecipeResult> CreateOrReplaceAsync(CreateOrReplaceRecipePayload payload, Guid? id, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeCommand command = new(payload, id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishRecipeCommand command = PublishRecipeCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishRecipeCommand command = string.IsNullOrWhiteSpace(language)
      ? PublishRecipeCommand.Invariant(id)
      : PublishRecipeCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadRecipeQuery query = new(id);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeModel?> SaveLocaleAsync(Guid id, string language, SaveRecipeLocalePayload payload, CancellationToken cancellationToken)
  {
    SaveRecipeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<SearchResults<RecipeModel>> SearchAsync(SearchRecipesPayload payload, CancellationToken cancellationToken)
  {
    SearchRecipesQuery query = new(payload);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public async Task<RecipeModel?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishRecipeCommand command = UnpublishRecipeCommand.All(id);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishRecipeCommand command = string.IsNullOrWhiteSpace(language)
      ? UnpublishRecipeCommand.Invariant(id)
      : UnpublishRecipeCommand.Locale(id, language);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> UpdateAsync(Guid id, UpdateRecipePayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeCommand command = new(id, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }

  public async Task<RecipeModel?> UpdateLocaleAsync(Guid id, string language, UpdateRecipeLocalePayload payload, CancellationToken cancellationToken)
  {
    UpdateRecipeLocaleCommand command = new(id, language, payload);
    return await _commandBus.ExecuteAsync(command, cancellationToken);
  }
}

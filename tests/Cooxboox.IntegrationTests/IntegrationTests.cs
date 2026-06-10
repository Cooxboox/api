using Bogus;
using Cooxboox.Builders;
using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Infrastructure;
using Cooxboox.PostgreSQL;
using Krakenar.Client.Users;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.CQRS;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Cooxboox;

public abstract class IntegrationTests : IAsyncLifetime
{
  protected virtual IConfiguration Configuration { get; }
  protected virtual TestContext Context { get; }
  protected virtual IServiceProvider ServiceProvider { get; }

  protected virtual Faker Faker { get; } = new();
  protected virtual Mock<IUserClient> UserClient { get; } = new();

  protected Actor Actor
  {
    get
    {
      if (Context.User is null)
      {
        throw new InvalidOperationException("An authenticated user is required.");
      }
      return new Actor(Context.User);
    }
  }

  protected IntegrationTests()
  {
    Configuration = BuildConfiguration();
    Context = new TestContext(Faker);
    ServiceProvider = BuildServiceProvider();
  }

  protected virtual IConfiguration BuildConfiguration() => new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

  protected virtual IServiceProvider BuildServiceProvider()
  {
    ServiceCollection services = new();

    services.AddSingleton(Configuration);
    services.AddSingleton<IContext>(Context);

    services.AddCooxbooxCore();
    services.AddCooxbooxInfrastructure();

    string connectionString = (EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Cooxboox") ?? Configuration.GetConnectionString("PostgreSQL") ?? string.Empty)
      .Replace("{Database}", GetType().Name);
    services.AddCooxbooxPostgreSQL(connectionString);

    services.AddSingleton(UserClient.Object);

    return services.BuildServiceProvider();
  }

  public virtual async Task InitializeAsync()
  {
    await MigrateDatabaseAsync();
    await ClearDatabaseAsync();
    await InitializeDatabaseAsync();
  }
  protected virtual async Task MigrateDatabaseAsync()
  {
    ICommandBus commandBus = ServiceProvider.GetRequiredService<ICommandBus>();
    await commandBus.ExecuteAsync(new MigrateDatabaseCommand());
  }
  protected virtual async Task ClearDatabaseAsync()
  {
    CooxbooxContext cooxboox = ServiceProvider.GetRequiredService<CooxbooxContext>();
    StringBuilder query = new();
    TableId[] tables =
    [
      Infrastructure.Db.RecipeCategories.Table,
      Infrastructure.Db.RecipeTypes.Table,
      Infrastructure.Db.Ingredients.Table,
      Infrastructure.Db.IngredientCategories.Table,
      Infrastructure.Db.IngredientTypes.Table,
      Infrastructure.Db.Kitchens.Table,
      EventDb.Events.Table,
      EventDb.Streams.Table
    ];
    foreach (TableId table in tables)
    {
      query.Append(new PostgresDeleteBuilder(table).Build().Text).Append(';').AppendLine();
    }
    await cooxboox.Database.ExecuteSqlRawAsync(query.ToString());
  }
  protected virtual async Task InitializeDatabaseAsync()
  {
    Context.User = new UserBuilder(Faker).Build();
    UserClient.Setup(x => x.SearchAsync(
      It.Is<SearchUsersPayload>(p => p.HasPassword == null && p.IsDisabled == null && p.IsConfirmed == null && p.HasAuthenticated == null && p.RoleId == null
        && p.Ids.Single() == Context.User.Id && p.Search.Terms.Count == 0 && p.Skip == 0 && p.Limit == 0),
      It.IsAny<CancellationToken>())).ReturnsAsync(new SearchResults<User>([Context.User]));

    Context.Kitchen = new KitchenBuilder(Faker).WithOwner(Context.User).Build();
    IKitchenRepository kitchenRepository = ServiceProvider.GetRequiredService<IKitchenRepository>();
    await kitchenRepository.SaveAsync(Context.Kitchen);
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}

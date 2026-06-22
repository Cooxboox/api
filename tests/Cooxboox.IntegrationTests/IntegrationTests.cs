using Bogus;
using Cooxboox.Builders;
using Cooxboox.Core;
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

  protected virtual Mock<IUserClient> UserClient { get; } = new();

  protected virtual Actor Actor
  {
    get
    {
      User user = Context.User ?? throw new InvalidOperationException("An authenticated user is required.");
      return new Actor(user);
    }
  }
  protected virtual Faker Faker { get; } = new();

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
    services.AddLogging();

    services.AddSingleton(Configuration);
    services.AddSingleton<IContext>(Context);

    services.AddCooxbooxCore();
    services.AddCooxbooxInfrastructure();

    string connectionString = (EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Cooxboox") ?? Configuration.GetConnectionString("PostgreSQL"))
      ?.Replace("{Database}", GetType().Name)
      ?? throw new InvalidOperationException("The PostgreSQL connection string was not found.");
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
    StringBuilder command = new();
    TableId[] tables =
    [
      Infrastructure.Db.Kitchens.Table,
      Infrastructure.Db.History.Table
    ];
    foreach (TableId table in tables)
    {
      command.Append(new PostgresDeleteBuilder(table).Build().Text).Append(';').AppendLine();
    }
    await cooxboox.Database.ExecuteSqlRawAsync(command.ToString());
  }
  protected virtual async Task InitializeDatabaseAsync()
  {
    Context.Database = ServiceProvider.GetRequiredService<CooxbooxContext>();
    Context.User = new UserBuilder(Faker).Build();
    UserClient.Setup(x => x.SearchAsync(
      It.Is<SearchUsersPayload>(p => p.Ids.Single() == Context.UserId),
      It.IsAny<CancellationToken>())).ReturnsAsync(new SearchResults<User>([Context.User]));
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}

using Bogus;
using Cooxboox.Core;
using Cooxboox.Infrastructure;
using Krakenar.Contracts.Users;

namespace Cooxboox;

public class TestContext : IContext
{
  private readonly Faker _faker;

  public CooxbooxContext? Database { get; set; }

  public User? User { get; set; }
  public Guid UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");

  public TestContext(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    if (Database is null)
    {
      throw new InvalidOperationException("The database context has not been initialized.");
    }
    return await Database.SaveChangesAsync(cancellationToken);
  }

  public Guid? TryGetUserId() => User?.Id;
}

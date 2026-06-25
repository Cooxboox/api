using Bogus;
using Cooxboox.Core;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Infrastructure;
using Krakenar.Contracts.Users;

namespace Cooxboox;

public class TestContext : IContext
{
  private readonly Faker _faker;

  public TestContext(Faker? faker = null)
  {
    _faker = faker ?? new();
  }

  public CooxbooxContext? Database { get; set; }

  public User? User { get; set; }
  public Guid UserId => TryGetUserId() ?? throw new InvalidOperationException("An authenticated user is required.");

  public KitchenModel? Kitchen { get; set; }
  public Guid KitchenId => TryGetKitchenId() ?? throw new InvalidOperationException("A kitchen is required.");
  public bool IsKitchenOwner => User is not null && Kitchen is not null && Kitchen.Owner.Id == User.Id;

  public Guid? TryGetKitchenId() => Kitchen?.Id;
  public Guid? TryGetUserId() => User?.Id;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    if (Database is null)
    {
      throw new InvalidOperationException("The database context has not been initialized.");
    }
    return await Database.SaveChangesAsync(cancellationToken);
  }
}

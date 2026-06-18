using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Core.Kitchens;

public interface IKitchenManager
{
  Task EnsureUniticityAsync(Kitchen kitchen, CancellationToken cancellationToken = default);
}

internal class KitchenManager : IKitchenManager
{
  private readonly IDbContext _database;

  public KitchenManager(IDbContext database)
  {
    _database = database;
  }

  public async Task EnsureUniticityAsync(Kitchen kitchen, CancellationToken cancellationToken)
  {
    if (kitchen.Slug is not null)
    {
      Guid? kitchenId = await _database.Kitchens
        .Where(x => x.Slug == kitchen.Slug)
        .Select(x => (Guid?)x.EntityId)
        .SingleOrDefaultAsync(cancellationToken);
      if (kitchenId.HasValue && !kitchenId.Value.Equals(kitchen.EntityId))
      {
        throw new KitchenSlugAlreadyUsedException(kitchen, kitchenId.Value);
      }
    }
  }
}

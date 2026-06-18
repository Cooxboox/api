using Cooxboox.Core.Kitchens;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Core;

public interface IDbContext
{
  DbSet<Kitchen> Kitchens { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

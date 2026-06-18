using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext, IDbContext
{
  protected CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  public DbSet<Kitchen> Kitchens => Set<Kitchen>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}

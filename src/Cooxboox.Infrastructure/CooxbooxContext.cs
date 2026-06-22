using Cooxboox.Core.Kitchens;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext
{
  public CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  internal DbSet<ChangeRecord> ChangeRecords => Set<ChangeRecord>();
  internal DbSet<Kitchen> Kitchens => Set<Kitchen>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}

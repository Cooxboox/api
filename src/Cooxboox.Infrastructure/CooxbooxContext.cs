using Cooxboox.Core.Kitchens;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext
{
  public CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  internal DbSet<HistoryRecord> History => Set<HistoryRecord>();
  internal DbSet<Kitchen> Kitchens => Set<Kitchen>();
  internal DbSet<KitchenLocale> KitchenLocales => Set<KitchenLocale>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}

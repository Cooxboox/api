using Cooxboox.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext
{
  internal const string? Schema = null;

  public CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  internal DbSet<KitchenEntity> Kitchens => Set<KitchenEntity>();
  internal DbSet<KitchenLocaleEntity> KitchenLocales => Set<KitchenLocaleEntity>();
  internal DbSet<PublishedKitchenLocaleEntity> PublishedKitchenLocales => Set<PublishedKitchenLocaleEntity>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}

using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Infrastructure;

public class CooxbooxContext : DbContext
{
  internal const string? Schema = null;

  public CooxbooxContext(DbContextOptions<CooxbooxContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}

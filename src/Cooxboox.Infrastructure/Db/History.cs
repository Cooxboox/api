using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

public static class History
{
  public static readonly TableId Table = new(nameof(CooxbooxContext.History));
}

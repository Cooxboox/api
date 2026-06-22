using Logitar.Data;

namespace Cooxboox.Infrastructure.Db;

public static class Kitchens
{
  public static readonly TableId Table = new(Schemas.Content, nameof(CooxbooxContext.Kitchens), alias: null);
}

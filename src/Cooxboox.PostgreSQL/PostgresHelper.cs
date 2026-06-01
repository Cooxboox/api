using Cooxboox.Infrastructure;
using Logitar.Data;
using Logitar.Data.PostgreSQL;

namespace Cooxboox.PostgreSQL;

internal class PostgresHelper : SqlHelper
{
  public override IQueryBuilder Query(TableId table) => PostgresQueryBuilder.From(table);

  protected override ConditionalOperator CreateOperator(string pattern) => PostgresOperators.IsLikeInsensitive(pattern);
}

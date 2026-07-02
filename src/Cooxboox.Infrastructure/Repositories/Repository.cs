using Cooxboox.Core;

namespace Cooxboox.Infrastructure.Repositories;

internal abstract class Repository
{
  protected virtual CooxbooxContext Database { get; set; }

  protected Repository(CooxbooxContext database)
  {
    Database = database;
  }

  protected virtual void RecordChange(ChangeEvent @event)
  {
    HistoryRecord record = new(@event);
    Database.History.Add(record);
  }
}

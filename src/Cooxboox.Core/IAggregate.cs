namespace Cooxboox.Core;

public interface IAggregate
{
  Guid EntityId { get; }
  long Version { get; }
  Guid CreatedBy { get; }
  DateTime CreatedOn { get; }
  Guid UpdatedBy { get; }
  DateTime UpdatedOn { get; }
}

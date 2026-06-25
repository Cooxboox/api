namespace Cooxboox.Core;

public interface IPublishable
{
  ContentStatus Status { get; }
  Guid? PublishedBy { get; }
  DateTime? PublishedOn { get; }
}

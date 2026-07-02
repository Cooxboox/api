using Krakenar.Contracts.Actors;

namespace Cooxboox.Core;

public interface IPublishable
{
  ContentStatus Status { get; }
  Guid? PublishedBy { get; }
  DateTime? PublishedOn { get; }
}

public interface IPublishableModel
{
  ContentStatus Status { get; set; }
  Actor? PublishedBy { get; set; }
  DateTime? PublishedOn { get; set; }
}

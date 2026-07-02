using Krakenar.Contracts.Actors;

namespace Cooxboox.Core;

public interface IPublishableModel
{
  ContentStatus Status { get; set; }
  Actor? PublishedBy { get; set; }
  DateTime? PublishedOn { get; set; }
}

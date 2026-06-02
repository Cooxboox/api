using Krakenar.Contracts;
using Logitar.EventSourcing;

namespace Cooxboox.Core;

public interface IContext
{
  ActorId? ActorId { get; }
  UserId UserId { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();
}

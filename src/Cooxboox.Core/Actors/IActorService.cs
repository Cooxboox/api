using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken = default);
}

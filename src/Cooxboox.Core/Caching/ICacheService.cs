using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Caching;

public interface ICacheService
{
  Guid? RealmId { get; set; }

  Actor? GetActor(ActorId id);
  void RemoveActor(ActorId id);
  void SetActor(Actor actor);
}

using Krakenar.Contracts.Actors;

namespace Cooxboox.Core.Caching;

public interface ICacheService
{
  Actor? GetActor(Guid id);
  void RemoveActor(Guid id);
  void SetActor(Actor actor);
}

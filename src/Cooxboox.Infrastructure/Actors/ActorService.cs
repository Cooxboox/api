using Cooxboox.Core.Actors;
using Cooxboox.Core.Caching;
using Cooxboox.Core.Identity;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Actors;

internal class ActorService : IActorService
{
  private readonly ICacheService _cacheService;
  private readonly IUserGateway _userGateway;

  public ActorService(ICacheService cacheService, IUserGateway userGateway)
  {
    _cacheService = cacheService;
    _userGateway = userGateway;
  }

  public async Task<IReadOnlyDictionary<ActorId, Actor>> FindAsync(IEnumerable<ActorId> actorIds, CancellationToken cancellationToken)
  {
    int count = actorIds.Count();
    Dictionary<ActorId, Actor> actors = new(count);
    HashSet<Guid> userIds = new(count);

    if (count > 0)
    {
      foreach (ActorId actorId in actorIds)
      {
        Actor? actor = _cacheService.GetActor(actorId);
        if (actor is null)
        {
          actor = actorId.ToActor();
          if (actor.Type == ActorType.User)
          {
            userIds.Add(actor.Id);
          }
        }
        actors[actorId] = actor;
      }

      if (userIds.Count > 0)
      {
        IReadOnlyCollection<User> users = await _userGateway.FindAsync(userIds, cancellationToken);
        foreach (User user in users)
        {
          Actor actor = new(user);
          ActorId actorId = actor.ToActorId();
          actors[actorId] = actor;
        }
      }

      foreach (Actor actor in actors.Values)
      {
        _cacheService.SetActor(actor);
      }
    }

    return actors.AsReadOnly();
  }
}

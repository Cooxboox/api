using Cooxboox.Core.Caching;
using Krakenar.Contracts.Actors;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Actors;

public interface IActorService
{
  Task<IReadOnlyDictionary<Guid, Actor>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

internal class ActorService : IActorService
{
  public static void Register(IServiceCollection services)
  {
    services.AddSingleton<IActorService, ActorService>();
  }

  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public async Task<IReadOnlyDictionary<Guid, Actor>> FindAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<Guid, Actor> actors = new(capacity);

    if (capacity > 0)
    {
      HashSet<Guid> missingIds = new(capacity);
      foreach (Guid id in ids)
      {
        Actor? actor = _cacheService.GetActor(id);
        if (actor is null)
        {
          missingIds.Add(id);
        }
        else
        {
          actors[id] = actor;
        }
      }

      if (missingIds.Count > 0)
      {
        // TODO(fpion): retrieve from Krakenar!
      }

      foreach (Actor actor in actors.Values)
      {
        _cacheService.SetActor(actor);
      }
    }

    return actors.AsReadOnly();
  }
}

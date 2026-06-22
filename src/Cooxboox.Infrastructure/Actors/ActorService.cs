using Cooxboox.Core.Caching;
using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;
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
    Dictionary<Guid, User> users = new(capacity);

    if (capacity > 0)
    {
      HashSet<Guid> missingIds = new(capacity);
      foreach (Guid id in ids)
      {
        User? user = _cacheService.GetUser(id);
        if (user is null)
        {
          missingIds.Add(id);
        }
        else
        {
          users[id] = user;
        }
      }

      if (missingIds.Count > 0)
      {
        // TODO(fpion): retrieve from Krakenar!
      }

      foreach (User user in users.Values)
      {
        _cacheService.SetUser(user);
      }
    }

    return users.ToDictionary(x => x.Key, x => new Actor(x.Value)).AsReadOnly();
  }
}

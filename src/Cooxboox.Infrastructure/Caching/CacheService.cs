using Cooxboox.Core.Actors;
using Cooxboox.Core.Caching;
using Cooxboox.Infrastructure.Settings;
using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;
using Microsoft.Extensions.Caching.Memory;

namespace Cooxboox.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private const string RealmIdKey = "RealmId";

  private readonly IMemoryCache _cache;
  private readonly CachingSettings _settings;

  public Guid? RealmId
  {
    get => _cache.TryGetValue(RealmIdKey, out object? value) ? (Guid?)value : null;
    set
    {
      if (value.HasValue)
      {
        _cache.Set(RealmIdKey, value.Value);
      }
      else
      {
        _cache.Remove(RealmIdKey);
      }
    }
  }

  public CacheService(IMemoryCache cache, CachingSettings settings)
  {
    _cache = cache;
    _settings = settings;
  }

  public Actor? GetActor(ActorId id)
  {
    string key = GetActorKey(id);
    return _cache.TryGetValue(key, out object? value) ? (Actor?)value : null;
  }
  public void RemoveActor(ActorId id)
  {
    string key = GetActorKey(id);
    _cache.Remove(key);
  }
  public void SetActor(Actor actor)
  {
    string key = GetActorKey(actor.ToActorId());
    _cache.Set(key, actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id={id}";
}

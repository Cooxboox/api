using Cooxboox.Core.Caching;
using Krakenar.Contracts.Actors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cooxboox.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  public static void Register(IServiceCollection services)
  {
    services.AddMemoryCache();
    services.AddSingleton(serviceProvider => CachingSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddSingleton<ICacheService, CacheService>();
  }

  private readonly IMemoryCache _cache;
  private readonly CachingSettings _settings;

  public CacheService(IMemoryCache cache, CachingSettings settings)
  {
    _cache = cache;
    _settings = settings;
  }

  public Actor? GetActor(Guid id)
  {
    string key = GetActorKey(id);
    return _cache.TryGetValue(key, out object? value) ? (Actor?)value : null;
  }
  public void RemoveActor(Guid id)
  {
    string key = GetActorKey(id);
    _cache.Remove(key);
  }
  public void SetActor(Actor actor)
  {
    if (actor.Type != ActorType.User)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    string key = GetActorKey(actor.Id);
    _cache.Set(key, actor, _settings.ActorLifetime);
  }
  private static string GetActorKey(Guid id) => $"Actor.Id:{id}";
}

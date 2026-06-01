using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public KitchenModel ToKitchen(KitchenEntity source)
  {
    KitchenModel destination = new()
    {
      Id = source.UniqueId,
      Owner = FindActor(source.OwnerId),
      Confidentiality = source.Confidentiality,
      Name = source.Name,
      Slug = source.Slug
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;
    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();
    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? actorId) => FindActor(actorId is null ? null : new ActorId(actorId));
  private Actor FindActor(ActorId? actorId) => TryGetActor(actorId) ?? _system;
  private Actor? TryGetActor(string? actorId) => TryGetActor(actorId is null ? null : new ActorId(actorId));
  private Actor? TryGetActor(ActorId? actorId) => actorId.HasValue && _actors.TryGetValue(actorId.Value, out Actor? actor) ? actor : null;
}

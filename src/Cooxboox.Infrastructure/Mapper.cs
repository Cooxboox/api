using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;

namespace Cooxboox.Infrastructure;

internal class Mapper
{
  private readonly Dictionary<Guid, Actor> _actors = [];
  private readonly Actor _system = new();

  public Mapper()
  {
  }

  public Mapper(IEnumerable<KeyValuePair<Guid, Actor>> actors)
  {
    foreach (KeyValuePair<Guid, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public KitchenModel ToKitchen(Kitchen source)
  {
    KitchenModel destination = new()
    {
      Owner = FindActor(source.OwnerId),
      Confidentiality = source.Confidentiality,
      Name = source.Name,
      Slug = source.Slug,
      Notes = source.Notes
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Id = source.Id;
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(Guid? id) => TryGetActor(id) ?? _system;
  private Actor? TryGetActor(Guid? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}

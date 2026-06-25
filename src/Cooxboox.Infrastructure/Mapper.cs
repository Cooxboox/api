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
      Id = source.Id,
      Owner = FindActor(source.OwnerId),
      Confidentiality = source.Confidentiality,
      Name = source.Name,
      Slug = source.Slug,
      Notes = source.Notes,
      Status = source.Status,
      PublishedBy = TryGetActor(source.PublishedBy),
      PublishedOn = source.PublishedOn?.AsUniversalTime()
    };

    MapAggregate(source, destination);

    return destination;
  }

  // TODO(fpion): IPublishable

  private void MapAggregate(object? source, Aggregate destination)
  {
    if (source is IAuditable auditable)
    {
      destination.CreatedBy = FindActor(auditable.CreatedBy);
      destination.CreatedOn = auditable.CreatedOn.AsUniversalTime();
      destination.UpdatedBy = FindActor(auditable.UpdatedBy);
      destination.UpdatedOn = auditable.UpdatedOn.AsUniversalTime();
    }

    if (source is IVersioned versioned)
    {
      destination.Version = versioned.Version;
    }
  }

  private Actor FindActor(Guid? id) => TryGetActor(id) ?? _system;
  private Actor? TryGetActor(Guid? id) => id.HasValue && _actors.TryGetValue(id.Value, out Actor? actor) ? actor : null;
}

using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Actors;

public static class ActorExtensions
{
  private const char Separator = '|';
  private const string RealmKind = "Realm";

  public static Actor ToActor(this ActorId actorId)
  {
    string[] values = actorId.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException($"The actor identifier '{actorId}' is not valid.", nameof(actorId));
    }

    Entity? realm = values.Length == 2 ? Entity.Parse(values.First(), RealmKind) : null;

    Entity entity = Entity.Parse(values.Last());
    if (!Enum.TryParse(entity.Kind, out ActorType type) || !Enum.IsDefined(type))
    {
      throw new ArgumentException($"The actor type '{entity.Kind}' is not valid.", nameof(actorId));
    }

    return new Actor
    {
      RealmId = realm?.Id,
      Type = type,
      Id = entity.Id
    };
  }

  public static ActorId ToActorId(this Actor actor)
  {
    Entity? realm = actor.RealmId.HasValue ? new Entity(RealmKind, actor.RealmId.Value) : null;
    Entity entity = new(actor.Type.ToString(), actor.Id);
    return new ActorId(realm is null ? entity.ToString() : string.Join(Separator, realm, entity));
  }
}

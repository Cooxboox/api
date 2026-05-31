using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Actors;

public static class ActorExtensions
{
  private const char Separator = '|'; // TODO(fpion): refactor
  private const string RealmKind = "Realm"; // TODO(fpion): refactor

  public static ActorId GetActorId(this Actor actor)
  {
    Entity? realm = actor.RealmId.HasValue ? new Entity(RealmKind, actor.RealmId.Value) : null;
    Entity entity = new(actor.Type.ToString(), actor.Id);
    return new ActorId(realm is null ? entity.ToString() : string.Join(Separator, realm, entity));
  }
}

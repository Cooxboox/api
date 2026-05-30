using Logitar.EventSourcing;

namespace Cooxboox.Core.Validation;

public readonly struct UserId
{
  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public Guid? RealmId { get; }
  public Guid EntityId { get; }

  public UserId(ActorId actorId)
  {
    ActorId = actorId;

    // TODO(fpion): RealmId
    // TODO(fpion): EntityId
  }

  public UserId(string value) : this(new ActorId(value))
  {
  }

  public UserId(Guid entityId, Guid? realmId = null)
  {
    // TODO(fpion): ActorId

    RealmId = realmId;
    EntityId = entityId;
  }

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ActorId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

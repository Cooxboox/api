using Krakenar.Contracts.Users;
using Logitar.EventSourcing;

namespace Cooxboox.Core;

public readonly struct UserId
{
  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public UserId(ActorId actorId)
  {
    ActorId = actorId;

    // TODO(fpion): RealmId
    // TODO(fpion): EntityId
  }

  public UserId(string value) : this(new ActorId(value))
  {
  }

  public UserId(User user)
  {
    // TODO(fpion): implement
  }

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is UserId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

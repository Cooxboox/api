using Cooxboox.Core.Actors;
using Krakenar.Contracts.Actors;
using Logitar.EventSourcing;

namespace Cooxboox.Core;

public readonly struct UserId
{
  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public UserId(ActorId actorId)
  {
    if (actorId.GetActor().Type != ActorType.User)
    {
      throw new ArgumentException("The actor must be a User.", nameof(actorId));
    }

    ActorId = actorId;
  }

  public UserId(string value) : this(new ActorId(value))
  {
  }

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ActorId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

using Logitar.EventSourcing;

namespace Cooxboox.Core;

public readonly struct UserId
{
  private const char Separator = '|';
  private const string RealmKind = "Realm";
  private const string UserKind = "User";

  public ActorId ActorId { get; }
  public string Value => ActorId.Value;

  public Guid? RealmId { get; }
  public Guid EntityId { get; }

  public UserId(ActorId actorId)
  {
    ActorId = actorId;

    string[] values = actorId.Value.Split(Separator);
    if (values.Length > 2)
    {
      throw new ArgumentException("The actor identifier is not valid.", nameof(actorId));
    }
    RealmId = values.Length == 2 ? Entity.Parse(values.First(), RealmKind).Id : null;
    EntityId = Entity.Parse(values.Last(), UserKind).Id;
  }

  public UserId(string value) : this(new ActorId(value))
  {
  }

  public UserId(Guid entityId, Guid? realmId = null)
  {
    List<string> values = new(capacity: 2);
    if (realmId.HasValue)
    {
      values.Add(new Entity(RealmKind, realmId.Value).ToString());
    }
    values.Add(new Entity(UserKind, entityId).ToString());
    ActorId = new(string.Join(Separator, values));

    RealmId = realmId;
    EntityId = entityId;
  }

  public static bool operator ==(UserId left, UserId right) => left.Equals(right);
  public static bool operator !=(UserId left, UserId right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is ActorId id && id.Value == Value;
  public override int GetHashCode() => Value.GetHashCode();
  public override string ToString() => Value;
}

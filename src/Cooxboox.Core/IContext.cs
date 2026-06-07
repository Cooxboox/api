using Cooxboox.Core.Identity;
using Cooxboox.Core.Kitchens;
using Krakenar.Contracts;
using Logitar.EventSourcing;

namespace Cooxboox.Core;

public interface IContext
{
  ActorId? ActorId { get; }
  UserId UserId { get; }

  KitchenId KitchenId { get; }
  bool IsKitchenOwner { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();
}

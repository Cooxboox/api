using Cooxboox.Core.Identity;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenCreated(UserId OwnerId, Name Name) : DomainEvent;

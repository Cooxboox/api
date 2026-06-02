using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenDeleted : DomainEvent, IDeleteEvent;

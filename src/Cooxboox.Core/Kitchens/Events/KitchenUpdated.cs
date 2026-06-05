using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenUpdated(Name? Name) : DomainEvent;

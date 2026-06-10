using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenAnnotated(Notes? Notes) : DomainEvent;

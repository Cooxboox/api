using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenRenamed(Name Name) : DomainEvent;

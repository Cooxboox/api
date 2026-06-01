using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenConfidentialityChanged(Confidentiality Confidentiality) : DomainEvent;

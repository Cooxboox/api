using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenUnpublished(Language? Language) : DomainEvent;

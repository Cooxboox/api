using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenLocaleUnpublished(Language Language) : DomainEvent;

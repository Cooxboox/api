using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenLocalePublished(Language Language) : DomainEvent;

using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenLocaleChanged(Language Language, KitchenLocale Locale) : DomainEvent;

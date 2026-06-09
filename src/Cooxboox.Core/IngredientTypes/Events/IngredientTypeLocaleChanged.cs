using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeLocaleChanged(Language Language, IngredientTypeLocale Locale) : DomainEvent;

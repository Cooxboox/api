using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientLocaleChanged(Language Language, IngredientLocale Locale) : DomainEvent;

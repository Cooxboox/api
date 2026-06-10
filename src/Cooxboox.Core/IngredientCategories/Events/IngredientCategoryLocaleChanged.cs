using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryLocaleChanged(Language Language, IngredientCategoryLocale Locale) : DomainEvent;

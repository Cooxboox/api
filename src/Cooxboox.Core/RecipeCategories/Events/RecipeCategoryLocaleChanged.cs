using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryLocaleChanged(Language Language, RecipeCategoryLocale Locale) : DomainEvent;

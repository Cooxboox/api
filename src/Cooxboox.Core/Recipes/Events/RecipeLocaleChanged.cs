using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeLocaleChanged(Language Language, RecipeLocale Locale) : DomainEvent;

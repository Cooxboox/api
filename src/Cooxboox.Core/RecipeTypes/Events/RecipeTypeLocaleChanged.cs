using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeLocaleChanged(Language Language, RecipeTypeLocale Locale) : DomainEvent;

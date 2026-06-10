using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryLocaleRemoved(Language Language) : DomainEvent;

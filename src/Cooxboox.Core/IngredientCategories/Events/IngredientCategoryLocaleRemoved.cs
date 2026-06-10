using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryLocaleRemoved(Language Language) : DomainEvent;

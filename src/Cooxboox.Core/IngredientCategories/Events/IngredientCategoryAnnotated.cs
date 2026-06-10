using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryAnnotated(Notes? Notes) : DomainEvent;
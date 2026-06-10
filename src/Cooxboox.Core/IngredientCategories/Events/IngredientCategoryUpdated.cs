using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

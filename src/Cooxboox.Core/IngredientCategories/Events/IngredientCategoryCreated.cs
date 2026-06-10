using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryCreated(Name Name) : DomainEvent;

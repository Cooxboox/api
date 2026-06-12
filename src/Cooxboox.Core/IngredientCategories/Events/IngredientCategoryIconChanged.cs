using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryIconChanged(Icon? Icon) : DomainEvent;

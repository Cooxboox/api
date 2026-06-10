using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientCategories.Events;

public record IngredientCategoryDeleted : DomainEvent, IDeleteEvent;

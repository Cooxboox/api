using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientDeleted : DomainEvent, IDeleteEvent;

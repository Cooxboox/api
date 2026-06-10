using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientAnnotated(Notes? Notes) : DomainEvent;

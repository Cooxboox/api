using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

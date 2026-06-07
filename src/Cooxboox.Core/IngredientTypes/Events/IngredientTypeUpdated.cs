using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

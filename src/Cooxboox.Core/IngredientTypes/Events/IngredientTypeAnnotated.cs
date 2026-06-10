using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeAnnotated(Notes? Notes) : DomainEvent;
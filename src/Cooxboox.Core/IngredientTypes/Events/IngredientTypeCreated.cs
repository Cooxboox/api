using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeCreated(Name Name) : DomainEvent;

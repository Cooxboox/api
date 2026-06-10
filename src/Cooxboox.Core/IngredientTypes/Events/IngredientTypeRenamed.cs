using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeRenamed(Name Name) : DomainEvent;
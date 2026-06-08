using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeDeleted : DomainEvent, IDeleteEvent;

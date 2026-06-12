using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeIconChanged(Icon? Icon) : DomainEvent;

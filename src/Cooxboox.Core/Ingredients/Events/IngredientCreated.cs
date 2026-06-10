using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientCreated(Name Name) : DomainEvent;

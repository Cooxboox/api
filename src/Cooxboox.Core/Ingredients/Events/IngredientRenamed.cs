using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientRenamed(Name Name) : DomainEvent;

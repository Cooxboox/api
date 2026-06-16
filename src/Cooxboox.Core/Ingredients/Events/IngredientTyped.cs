using Cooxboox.Core.IngredientTypes;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientTyped(IngredientTypeId? IngredientTypeId) : DomainEvent;

using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientUnpublished(Language? Language) : DomainEvent;

using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientPublished(Language? Language) : DomainEvent;

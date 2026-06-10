using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients.Events;

public record IngredientLocaleRemoved(Language Language) : DomainEvent;

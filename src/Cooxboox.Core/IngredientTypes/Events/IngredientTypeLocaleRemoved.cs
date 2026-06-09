using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes.Events;

public record IngredientTypeLocaleRemoved(Language Language) : DomainEvent;

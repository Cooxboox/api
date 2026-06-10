using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeUnpublished(Language? Language) : DomainEvent;

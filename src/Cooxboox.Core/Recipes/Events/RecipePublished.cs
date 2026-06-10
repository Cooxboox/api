using Cooxboox.Core.Localization;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipePublished(Language? Language) : DomainEvent;

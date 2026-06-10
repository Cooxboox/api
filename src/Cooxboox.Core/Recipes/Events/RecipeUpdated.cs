using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

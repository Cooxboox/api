using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

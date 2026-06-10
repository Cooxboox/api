using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeAnnotated(Notes? Notes) : DomainEvent;
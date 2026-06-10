using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeCreated(Name Name) : DomainEvent;

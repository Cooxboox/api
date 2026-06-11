using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeRenamed(Name Name) : DomainEvent;
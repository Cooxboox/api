using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeDeleted : DomainEvent, IDeleteEvent;

using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeDeleted : DomainEvent, IDeleteEvent;

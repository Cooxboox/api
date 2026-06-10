using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryDeleted : DomainEvent, IDeleteEvent;

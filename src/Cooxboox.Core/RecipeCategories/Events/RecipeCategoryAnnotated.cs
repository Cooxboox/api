using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryAnnotated(Notes? Notes) : DomainEvent;
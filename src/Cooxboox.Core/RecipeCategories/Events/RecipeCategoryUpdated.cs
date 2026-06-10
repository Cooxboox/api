using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryUpdated(Name? Name, Optional<Notes>? Notes) : DomainEvent;

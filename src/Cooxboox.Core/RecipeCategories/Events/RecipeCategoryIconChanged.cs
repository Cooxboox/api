using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories.Events;

public record RecipeCategoryIconChanged(Icon? Icon) : DomainEvent;

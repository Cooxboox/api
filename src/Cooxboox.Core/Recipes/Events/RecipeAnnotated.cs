using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeAnnotated(Notes? Notes) : DomainEvent;
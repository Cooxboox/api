using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeRenamed(Name Name) : DomainEvent;
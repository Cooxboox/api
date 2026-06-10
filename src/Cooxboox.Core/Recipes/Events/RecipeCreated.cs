using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeCreated(Name Name) : DomainEvent;

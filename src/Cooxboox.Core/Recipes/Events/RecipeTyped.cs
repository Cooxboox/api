using Cooxboox.Core.RecipeTypes;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes.Events;

public record RecipeTyped(RecipeTypeId? RecipeTypeId) : DomainEvent;

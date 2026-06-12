using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes.Events;

public record RecipeTypeIconChanged(Icon? Icon) : DomainEvent;

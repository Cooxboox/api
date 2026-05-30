using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens.Events;

public record KitchenSlugChanged(Slug? Slug) : DomainEvent;

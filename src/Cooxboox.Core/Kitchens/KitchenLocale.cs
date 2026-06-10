using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public record KitchenLocale(MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

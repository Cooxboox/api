using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes;

public record IngredientTypeLocale(Name Name, Slug? Slug, MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Ingredients;

public record IngredientLocale(Name Name, Slug? Slug, MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

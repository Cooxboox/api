using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeTypes;

public record RecipeTypeLocale(Name Name, Slug? Slug, MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

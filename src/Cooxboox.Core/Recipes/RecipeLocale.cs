using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Recipes;

public record RecipeLocale(Name Name, Slug? Slug, MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

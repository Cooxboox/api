using Cooxboox.Core.Seo;
using Logitar.EventSourcing;

namespace Cooxboox.Core.RecipeCategories;

public record RecipeCategoryLocale(Name Name, Slug? Slug, MetaDescription? MetaDescription, HtmlContent? HtmlContent, Notes? Notes) : DomainEvent;

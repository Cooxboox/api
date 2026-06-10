using Cooxboox.Infrastructure.Converters;

namespace Cooxboox.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new HtmlContentConverter());
    SerializerOptions.Converters.Add(new IngredientCategoryIdConverter());
    SerializerOptions.Converters.Add(new IngredientTypeIdConverter());
    SerializerOptions.Converters.Add(new KitchenIdConverter());
    SerializerOptions.Converters.Add(new LanguageConverter());
    SerializerOptions.Converters.Add(new MetaDescriptionConverter());
    SerializerOptions.Converters.Add(new NameConverter());
    SerializerOptions.Converters.Add(new NotesConverter());
    SerializerOptions.Converters.Add(new RecipeCategoryIdConverter());
    SerializerOptions.Converters.Add(new RecipeTypeIdConverter());
    SerializerOptions.Converters.Add(new SlugConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
  }
}

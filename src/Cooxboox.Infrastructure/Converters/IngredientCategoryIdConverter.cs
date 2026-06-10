using Cooxboox.Core.IngredientCategories;

namespace Cooxboox.Infrastructure.Converters;

internal class IngredientCategoryIdConverter : JsonConverter<IngredientCategoryId>
{
  public override IngredientCategoryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new IngredientCategoryId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, IngredientCategoryId ingredientCategoryId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(ingredientCategoryId.Value);
  }
}

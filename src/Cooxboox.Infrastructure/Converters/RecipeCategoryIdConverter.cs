using Cooxboox.Core.RecipeCategories;

namespace Cooxboox.Infrastructure.Converters;

internal class RecipeCategoryIdConverter : JsonConverter<RecipeCategoryId>
{
  public override RecipeCategoryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RecipeCategoryId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RecipeCategoryId recipeCategoryId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(recipeCategoryId.Value);
  }
}

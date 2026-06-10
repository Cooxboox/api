using Cooxboox.Core.Recipes;

namespace Cooxboox.Infrastructure.Converters;

internal class RecipeIdConverter : JsonConverter<RecipeId>
{
  public override RecipeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RecipeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RecipeId recipeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(recipeId.Value);
  }
}

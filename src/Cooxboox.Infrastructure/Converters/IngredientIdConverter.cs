using Cooxboox.Core.Ingredients;

namespace Cooxboox.Infrastructure.Converters;

internal class IngredientIdConverter : JsonConverter<IngredientId>
{
  public override IngredientId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new IngredientId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, IngredientId ingredientId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(ingredientId.Value);
  }
}

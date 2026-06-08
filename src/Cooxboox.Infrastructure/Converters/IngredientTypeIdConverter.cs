using Cooxboox.Core.IngredientTypes;

namespace Cooxboox.Infrastructure.Converters;

internal class IngredientTypeIdConverter : JsonConverter<IngredientTypeId>
{
  public override IngredientTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new IngredientTypeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, IngredientTypeId ingredientTypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(ingredientTypeId.Value);
  }
}

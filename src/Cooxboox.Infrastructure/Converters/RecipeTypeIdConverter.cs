using Cooxboox.Core.RecipeTypes;

namespace Cooxboox.Infrastructure.Converters;

internal class RecipeTypeIdConverter : JsonConverter<RecipeTypeId>
{
  public override RecipeTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RecipeTypeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RecipeTypeId recipeTypeId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(recipeTypeId.Value);
  }
}

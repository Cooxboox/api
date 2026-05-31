using Cooxboox.Core.Kitchens;

namespace Cooxboox.Infrastructure.Converters;

internal class KitchenIdConverter : JsonConverter<KitchenId>
{
  public override KitchenId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new KitchenId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, KitchenId kitchenId, JsonSerializerOptions options)
  {
    writer.WriteStringValue(kitchenId.Value);
  }
}

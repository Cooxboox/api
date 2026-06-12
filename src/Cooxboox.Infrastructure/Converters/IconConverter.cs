using Cooxboox.Core;

namespace Cooxboox.Infrastructure.Converters;

internal class IconConverter : JsonConverter<Icon>
{
  public override Icon? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Icon.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Icon icon, JsonSerializerOptions options)
  {
    writer.WriteStringValue(icon.Value);
  }
}

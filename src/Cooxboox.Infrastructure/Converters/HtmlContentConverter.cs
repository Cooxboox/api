using Cooxboox.Core;

namespace Cooxboox.Infrastructure.Converters;

internal class HtmlContentConverter : JsonConverter<HtmlContent>
{
  public override HtmlContent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return HtmlContent.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, HtmlContent htmlContent, JsonSerializerOptions options)
  {
    writer.WriteStringValue(htmlContent.Value);
  }
}

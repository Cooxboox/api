using Cooxboox.Core.Seo;

namespace Cooxboox.Infrastructure.Converters;

internal class MetaDescriptionConverter : JsonConverter<MetaDescription>
{
  public override MetaDescription? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return MetaDescription.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, MetaDescription metaDescription, JsonSerializerOptions options)
  {
    writer.WriteStringValue(metaDescription.Value);
  }
}

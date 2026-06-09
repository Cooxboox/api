using Cooxboox.Core.Seo;

namespace Cooxboox.Infrastructure.Converters;

internal class SlugConverter : JsonConverter<Slug>
{
  public override Slug? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Slug.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Slug slug, JsonSerializerOptions options)
  {
    writer.WriteStringValue(slug.Value);
  }
}

using Cooxboox.Core.Localization;

namespace Cooxboox.Infrastructure.Converters;

internal class LanguageConverter : JsonConverter<Language>
{
  public override Language? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? code = reader.GetString();
    return string.IsNullOrWhiteSpace(code) ? null : new Language(code);
  }

  public override void Write(Utf8JsonWriter writer, Language language, JsonSerializerOptions options)
  {
    writer.WriteStringValue(language.Code);
  }
}

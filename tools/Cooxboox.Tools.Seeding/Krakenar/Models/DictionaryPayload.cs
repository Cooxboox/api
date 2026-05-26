using Krakenar.Contracts.Dictionaries;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record DictionaryPayload : CreateOrReplaceDictionaryPayload
{
  public Guid Id { get; set; }
}

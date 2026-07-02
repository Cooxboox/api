using Krakenar.Contracts.Fields;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record FieldDefinitionPayload : CreateOrReplaceFieldDefinitionPayload
{
  public Guid Id { get; set; }
}

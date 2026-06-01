using Krakenar.Contracts.Fields;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record FieldTypePayload : CreateOrReplaceFieldTypePayload
{
  public Guid Id { get; set; }
}

using Krakenar.Contracts.Contents;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record ContentTypePayload : CreateOrReplaceContentTypePayload
{
  public Guid Id { get; set; }

  public List<FieldDefinitionPayload> Fields { get; set; } = [];
}

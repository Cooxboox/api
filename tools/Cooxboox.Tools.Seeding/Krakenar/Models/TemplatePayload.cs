using Krakenar.Contracts.Templates;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record TemplatePayload : CreateOrReplaceTemplatePayload
{
  public Guid Id { get; set; }
}

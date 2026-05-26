using Krakenar.Contracts.Senders;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record SenderPayload : CreateOrReplaceSenderPayload
{
  public Guid Id { get; set; }
}

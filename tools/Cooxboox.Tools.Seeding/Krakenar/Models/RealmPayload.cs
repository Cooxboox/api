using Krakenar.Contracts.Realms;

namespace Cooxboox.Tools.Seeding.Krakenar.Models;

internal record RealmPayload : CreateOrReplaceRealmPayload
{
  public Guid Id { get; set; }
}

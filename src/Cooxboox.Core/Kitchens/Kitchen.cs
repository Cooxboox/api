using Logitar;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : IEntityProvider
{
  public const string EntityKind = "Kitchen";

  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public Guid OwnerId { get; private set; }
  public Confidentiality Confidentiality { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Slug { get; private set; }
  public string? Notes { get; private set; }

  public long Version { get; private set; }
  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ContentStatus Status { get; private set; }
  public Guid? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public Kitchen(
    Guid ownerId,
    string name,
    Guid? entityId = null,
    Confidentiality confidentiality = Confidentiality.Private,
    string? slug = null,
    string? notes = null,
    DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();

    EntityId = entityId ?? Guid.NewGuid();

    OwnerId = ownerId;
    Confidentiality = confidentiality;

    CreatedBy = ownerId;
    CreatedOn = createdOn.Value;

    Update(ownerId, name, slug, notes, createdOn);
  }

  private Kitchen()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    HashSet<Guid> userIds = new(capacity: 3);
    userIds.Add(CreatedBy);
    userIds.Add(UpdatedBy);
    if (PublishedBy.HasValue)
    {
      userIds.Add(PublishedBy.Value);
    }
    return userIds;
  }

  public Entity GetEntity() => new(EntityKind, EntityId);

  public void Update(Guid userId, string name, string? slug = null, string? notes = null, DateTime? updatedOn = null)
  {
    Name = name.Trim();
    Slug = SlugHelper.Format(slug);
    Notes = notes?.CleanTrim();

    Version++;
    UpdatedBy = userId;
    UpdatedOn = (updatedOn ?? DateTime.Now).AsUniversalTime();

    if (Status == ContentStatus.Latest)
    {
      Status = ContentStatus.Published;
    }
  }

  public override bool Equals(object? obj) => obj is Kitchen kitchen && kitchen.KitchenId == KitchenId;
  public override int GetHashCode() => KitchenId.GetHashCode();
  public override string ToString() => $"{Name} | {base.ToString()} (KitchenId={KitchenId})";
}

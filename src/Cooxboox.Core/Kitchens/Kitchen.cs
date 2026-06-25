using Logitar;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : IAuditable, IPublishable, IResource, IVersioned
{
  public int KitchenId { get; private set; }
  public Guid Id { get; private set; }

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

  public ResourceIdentifier Identifier => new(ResourceKind.Kitchen, Id);

  public Kitchen(
    Guid ownerId,
    string name,
    Guid? id = null,
    Confidentiality confidentiality = Confidentiality.Private,
    string? slug = null,
    string? notes = null,
    DateTime? createdOn = null)
  {
    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();

    Id = id ?? Guid.NewGuid();

    OwnerId = ownerId;

    CreatedBy = ownerId;
    CreatedOn = createdOn.Value;

    Update(confidentiality, name, slug, notes, ownerId, createdOn);
  }

  private Kitchen()
  {
  }

  public IReadOnlyCollection<Guid> GetUserIds()
  {
    HashSet<Guid> userIds = new(capacity: 4);
    userIds.Add(OwnerId);
    userIds.Add(CreatedBy);
    userIds.Add(UpdatedBy);
    if (PublishedBy.HasValue)
    {
      userIds.Add(PublishedBy.Value);
    }
    return userIds;
  }

  public void Update(Confidentiality confidentiality, string name, string? slug, string? notes, Guid userId, DateTime? updatedOn = null)
  {
    Confidentiality = confidentiality;

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

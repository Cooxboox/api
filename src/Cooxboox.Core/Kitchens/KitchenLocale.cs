using Logitar;

namespace Cooxboox.Core.Kitchens;

public class KitchenLocale : IAuditable, IPublishable
{
  public Kitchen? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public string? MetaDescription { get; private set; }
  public string? HtmlContent { get; private set; }

  public Guid CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public Guid UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ContentStatus Status { get; private set; }
  public Guid? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public KitchenLocale(
    Kitchen kitchen,
    string language,
    Guid userId,
    string? metaDescription = null,
    string? htmlContent = null,
    DateTime? createdOn = null)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    Language = language;

    MetaDescription = metaDescription?.CleanTrim();
    HtmlContent = htmlContent?.CleanTrim();

    createdOn = (createdOn ?? DateTime.Now).AsUniversalTime();
    CreatedBy = userId;
    CreatedOn = createdOn.Value;
    UpdatedBy = userId;
    UpdatedOn = createdOn.Value;
  }

  private KitchenLocale()
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

  public override bool Equals(object? obj) => obj is KitchenLocale locale && locale.KitchenId == KitchenId && locale.Language == Language;
  public override int GetHashCode() => HashCode.Combine(KitchenId, Language);
  public override string ToString() => $"{base.ToString()} (KitchenId={KitchenId}, Language={Language})";
}

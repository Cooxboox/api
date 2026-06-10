using Cooxboox.Core;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientCategoryLocaleEntity
{
  public IngredientCategoryEntity? IngredientCategory { get; private set; }
  public int IngredientCategoryId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Slug { get; private set; }
  public string? MetaDescription { get; private set; }
  public string? HtmlContent { get; private set; }
  public string? Notes { get; private set; }

  public long Version { get; private set; }
  public string? CreatedBy { get; private set; }
  public DateTime CreatedOn { get; private set; }
  public string? UpdatedBy { get; private set; }
  public DateTime UpdatedOn { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public IngredientCategoryLocaleEntity(IngredientCategoryEntity ingredientCategory, IngredientCategoryLocaleChanged @event)
  {
    IngredientCategory = ingredientCategory;
    IngredientCategoryId = ingredientCategory.IngredientCategoryId;
    Language = @event.Language.Code;

    Kitchen = ingredientCategory.Kitchen;
    KitchenId = ingredientCategory.KitchenId;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private IngredientCategoryLocaleEntity()
  {
  }

  public IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = new(capacity: 3);
    if (CreatedBy is not null)
    {
      actorIds.Add(new ActorId(CreatedBy));
    }
    if (UpdatedBy is not null)
    {
      actorIds.Add(new ActorId(UpdatedBy));
    }
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    return actorIds;
  }

  public void Publish(IngredientCategoryPublished @event)
  {
    Status = ContentStatus.Latest;
    PublishedVersion = Version;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Unpublish(IngredientCategoryUnpublished @event)
  {
    Status = ContentStatus.Unpublished;
    PublishedVersion = null;
    PublishedBy = null;
    PublishedOn = null;
  }

  public void Update(IngredientCategoryLocaleChanged @event)
  {
    IngredientCategoryLocale locale = @event.Locale;
    Name = locale.Name.Value;
    Slug = locale.Slug?.Value;
    MetaDescription = locale.MetaDescription?.Value;
    HtmlContent = locale.HtmlContent?.Value;
    Notes = locale.Notes?.Value;

    Version = @event.Version;
    UpdatedBy = @event.ActorId?.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();

    if (Status == ContentStatus.Latest)
    {
      Status = ContentStatus.Published;
    }
  }

  public override bool Equals(object? obj) => obj is IngredientCategoryLocaleEntity locale && locale.IngredientCategoryId == IngredientCategoryId && locale.Language == Language;
  public override int GetHashCode() => HashCode.Combine(IngredientCategoryId, Language);
  public override string ToString() => $"{Name} | {base.ToString()} (IngredientCategoryId={IngredientCategoryId}, Language={Language})";
}

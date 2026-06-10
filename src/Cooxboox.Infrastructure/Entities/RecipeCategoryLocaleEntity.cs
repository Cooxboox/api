using Cooxboox.Core;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class RecipeCategoryLocaleEntity
{
  public RecipeCategoryEntity? RecipeCategory { get; private set; }
  public int RecipeCategoryId { get; private set; }
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

  public RecipeCategoryLocaleEntity(RecipeCategoryEntity recipeCategory, RecipeCategoryLocaleChanged @event)
  {
    RecipeCategory = recipeCategory;
    RecipeCategoryId = recipeCategory.RecipeCategoryId;
    Language = @event.Language.Code;

    Kitchen = recipeCategory.Kitchen;
    KitchenId = recipeCategory.KitchenId;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private RecipeCategoryLocaleEntity()
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

  public void Publish(RecipeCategoryPublished @event)
  {
    Status = ContentStatus.Latest;
    PublishedVersion = Version;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Unpublish(RecipeCategoryUnpublished @event)
  {
    Status = ContentStatus.Unpublished;
    PublishedVersion = null;
    PublishedBy = null;
    PublishedOn = null;
  }

  public void Update(RecipeCategoryLocaleChanged @event)
  {
    RecipeCategoryLocale locale = @event.Locale;
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

  public override bool Equals(object? obj) => obj is RecipeCategoryLocaleEntity locale && locale.RecipeCategoryId == RecipeCategoryId && locale.Language == Language;
  public override int GetHashCode() => HashCode.Combine(RecipeCategoryId, Language);
  public override string ToString() => $"{Name} | {base.ToString()} (RecipeCategoryId={RecipeCategoryId}, Language={Language})";
}

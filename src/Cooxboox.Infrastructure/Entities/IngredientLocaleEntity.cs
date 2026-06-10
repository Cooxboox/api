using Cooxboox.Core;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientLocaleEntity
{
  public IngredientEntity? Ingredient { get; private set; }
  public int IngredientId { get; private set; }
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

  public IngredientLocaleEntity(IngredientEntity ingredient, IngredientLocaleChanged @event)
  {
    Ingredient = ingredient;
    IngredientId = ingredient.IngredientId;
    Language = @event.Language.Code;

    Kitchen = ingredient.Kitchen;
    KitchenId = ingredient.KitchenId;

    CreatedBy = @event.ActorId?.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(@event);
  }

  private IngredientLocaleEntity()
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

  public void Publish(IngredientPublished @event)
  {
    Status = ContentStatus.Latest;
    PublishedVersion = Version;
    PublishedBy = @event.ActorId?.Value;
    PublishedOn = @event.OccurredOn.AsUniversalTime();
  }

  public void Unpublish(IngredientUnpublished @event)
  {
    Status = ContentStatus.Unpublished;
    PublishedVersion = null;
    PublishedBy = null;
    PublishedOn = null;
  }

  public void Update(IngredientLocaleChanged @event)
  {
    IngredientLocale locale = @event.Locale;
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

  public override bool Equals(object? obj) => obj is IngredientLocaleEntity locale && locale.IngredientId == IngredientId && locale.Language == Language;
  public override int GetHashCode() => HashCode.Combine(IngredientId, Language);
  public override string ToString() => $"{Name} | {base.ToString()} (IngredientId={IngredientId}, Language={Language})";
}

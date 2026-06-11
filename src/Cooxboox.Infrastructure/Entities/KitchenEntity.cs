using Cooxboox.Core;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenEntity : AggregateEntity
{
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string OwnerId { get; private set; } = string.Empty;
  public Confidentiality Confidentiality { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Slug { get; private set; }
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<IngredientCategoryEntity> IngredientCategories { get; private set; } = [];
  public List<IngredientCategoryLocaleEntity> IngredientCategoryLocales { get; private set; } = [];
  public List<IngredientEntity> Ingredients { get; private set; } = [];
  public List<IngredientLocaleEntity> IngredientLocales { get; private set; } = [];
  public List<IngredientTypeEntity> IngredientTypes { get; private set; } = [];
  public List<IngredientTypeLocaleEntity> IngredientTypeLocales { get; private set; } = [];
  public List<KitchenLocaleEntity> Locales { get; private set; } = [];
  public List<RecipeCategoryEntity> RecipeCategories { get; private set; } = [];
  public List<RecipeCategoryLocaleEntity> RecipeCategoryLocales { get; private set; } = [];
  public List<RecipeEntity> Recipes { get; private set; } = [];
  public List<RecipeLocaleEntity> RecipeLocales { get; private set; } = [];
  public List<RecipeTypeEntity> RecipeTypes { get; private set; } = [];
  public List<RecipeTypeLocaleEntity> RecipeTypeLocales { get; private set; } = [];

  public KitchenEntity(KitchenCreated @event) : base(@event)
  {
    EntityId = new KitchenId(@event.StreamId).EntityId;

    OwnerId = @event.OwnerId.Value;

    Name = @event.Name.Value;
  }

  private KitchenEntity() : base()
  {
  }

  public void Annotate(KitchenAnnotated @event)
  {
    UpdateInvariant(@event);

    Notes = @event.Notes?.Value;
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    actorIds.Add(new ActorId(OwnerId));
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (KitchenLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(KitchenPublished @event)
  {
    base.Update(@event);

    if (@event.Language is null)
    {
      Status = ContentStatus.Latest;
      PublishedVersion = Version;
      PublishedBy = @event.ActorId?.Value;
      PublishedOn = @event.OccurredOn.AsUniversalTime();
    }
    else
    {
      KitchenLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public KitchenLocaleEntity? RemoveLocale(KitchenLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void Rename(KitchenRenamed @event)
  {
    UpdateInvariant(@event);

    Name = @event.Name.Value;
  }

  public void SetLocale(KitchenLocaleChanged @event)
  {
    base.Update(@event);

    KitchenLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new KitchenLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void SetSlug(KitchenSlugChanged @event)
  {
    UpdateInvariant(@event);

    Slug = @event.Slug?.Value;
  }

  public void Unpublish(KitchenUnpublished @event)
  {
    base.Update(@event);

    if (@event.Language is null)
    {
      Status = ContentStatus.Unpublished;
      PublishedVersion = null;
      PublishedBy = null;
      PublishedOn = null;
    }
    else
    {
      KitchenLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  private KitchenLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The kitchen '{this}' locale '{language}' was not found.");
  private KitchenLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  private void UpdateInvariant(DomainEvent @event)
  {
    base.Update(@event);

    if (Status == ContentStatus.Latest)
    {
      Status = ContentStatus.Published;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

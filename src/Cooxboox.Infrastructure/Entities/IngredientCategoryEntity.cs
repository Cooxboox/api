using Cooxboox.Core;
using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientCategoryEntity : AggregateEntity
{
  public int IngredientCategoryId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<IngredientCategoryLocaleEntity> Locales { get; private set; } = [];

  public IngredientCategoryEntity(KitchenEntity kitchen, IngredientCategoryCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new IngredientCategoryId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private IngredientCategoryEntity() : base()
  {
  }

  public void Annotate(IngredientCategoryAnnotated @event)
  {
    base.Update(@event);

    Notes = @event.Notes?.Value;

    // TODO(fpion): invariant status
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (IngredientCategoryLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(IngredientCategoryPublished @event)
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
      IngredientCategoryLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public IngredientCategoryLocaleEntity? RemoveLocale(IngredientCategoryLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void Rename(IngredientCategoryRenamed @event)
  {
    base.Update(@event);

    Name = @event.Name.Value;

    // TODO(fpion): invariant status
  }

  public void SetLocale(IngredientCategoryLocaleChanged @event)
  {
    base.Update(@event);

    IngredientCategoryLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new IngredientCategoryLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void Unpublish(IngredientCategoryUnpublished @event)
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
      IngredientCategoryLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  private IngredientCategoryLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The ingredient category '{this}' locale '{language}' was not found.");
  private IngredientCategoryLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()}";
}

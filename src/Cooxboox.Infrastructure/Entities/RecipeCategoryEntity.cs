using Cooxboox.Core;
using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class RecipeCategoryEntity : AggregateEntity
{
  public int RecipeCategoryId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<RecipeCategoryLocaleEntity> Locales { get; private set; } = [];

  public RecipeCategoryEntity(KitchenEntity kitchen, RecipeCategoryCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new RecipeCategoryId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private RecipeCategoryEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (RecipeCategoryLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(RecipeCategoryPublished @event)
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
      RecipeCategoryLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public void Unpublish(RecipeCategoryUnpublished @event)
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
      RecipeCategoryLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  public RecipeCategoryLocaleEntity? RemoveLocale(RecipeCategoryLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void SetLocale(RecipeCategoryLocaleChanged @event)
  {
    base.Update(@event);

    RecipeCategoryLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new RecipeCategoryLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void Update(RecipeCategoryUpdated @event)
  {
    base.Update(@event);

    if (@event.Name is not null)
    {
      Name = @event.Name.Value;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value?.Value;
    }
  }

  private RecipeCategoryLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The recipe category '{this}' locale '{language}' was not found.");
  private RecipeCategoryLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()}";
}

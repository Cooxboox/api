using Cooxboox.Core;
using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.RecipeTypes.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class RecipeTypeEntity : AggregateEntity
{
  public int RecipeTypeId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<RecipeTypeLocaleEntity> Locales { get; private set; } = [];

  public RecipeTypeEntity(KitchenEntity kitchen, RecipeTypeCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new RecipeTypeId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private RecipeTypeEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (RecipeTypeLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(RecipeTypePublished @event)
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
      RecipeTypeLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public void Unpublish(RecipeTypeUnpublished @event)
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
      RecipeTypeLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  public RecipeTypeLocaleEntity? RemoveLocale(RecipeTypeLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void SetLocale(RecipeTypeLocaleChanged @event)
  {
    base.Update(@event);

    RecipeTypeLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new RecipeTypeLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void Update(RecipeTypeUpdated @event)
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

  private RecipeTypeLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The recipe type '{this}' locale '{language}' was not found.");
  private RecipeTypeLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()}";
}

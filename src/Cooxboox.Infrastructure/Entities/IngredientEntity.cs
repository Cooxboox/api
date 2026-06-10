using Cooxboox.Core;
using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientEntity : AggregateEntity
{
  public int IngredientId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<IngredientLocaleEntity> Locales { get; private set; } = [];

  public IngredientEntity(KitchenEntity kitchen, IngredientCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new IngredientId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private IngredientEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (IngredientLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(IngredientPublished @event)
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
      IngredientLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public void Unpublish(IngredientUnpublished @event)
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
      IngredientLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  public IngredientLocaleEntity? RemoveLocale(IngredientLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void SetLocale(IngredientLocaleChanged @event)
  {
    base.Update(@event);

    IngredientLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new IngredientLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void Update(IngredientUpdated @event)
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

  private IngredientLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The ingredient '{this}' locale '{language}' was not found.");
  private IngredientLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()}";
}

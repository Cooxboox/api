using Cooxboox.Core;
using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Events;
using Cooxboox.Core.Localization;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientTypeEntity : AggregateEntity
{
  public int IngredientTypeId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Icon { get; private set; }
  public string? Notes { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<IngredientTypeLocaleEntity> Locales { get; private set; } = [];

  public IngredientTypeEntity(KitchenEntity kitchen, IngredientTypeCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new IngredientTypeId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private IngredientTypeEntity() : base()
  {
  }

  public void Annotate(IngredientTypeAnnotated @event)
  {
    UpdateInvariant(@event);

    Notes = @event.Notes?.Value;
  }

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    if (PublishedBy is not null)
    {
      actorIds.Add(new ActorId(PublishedBy));
    }
    foreach (IngredientTypeLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(IngredientTypePublished @event)
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
      IngredientTypeLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public IngredientTypeLocaleEntity? RemoveLocale(IngredientTypeLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void Rename(IngredientTypeRenamed @event)
  {
    UpdateInvariant(@event);

    Name = @event.Name.Value;
  }

  public void SetIcon(IngredientTypeIconChanged @event)
  {
    UpdateInvariant(@event);

    Icon = @event.Icon?.Value;
  }

  public void SetLocale(IngredientTypeLocaleChanged @event)
  {
    base.Update(@event);

    IngredientTypeLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new IngredientTypeLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }
  public void Unpublish(IngredientTypeUnpublished @event)
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
      IngredientTypeLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  private IngredientTypeLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The ingredient type '{this}' locale '{language}' was not found.");
  private IngredientTypeLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

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

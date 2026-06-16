using Cooxboox.Core;
using Cooxboox.Core.Localization;
using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Events;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Infrastructure.Entities;

internal class RecipeEntity : AggregateEntity
{
  public int RecipeId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

  public RecipeTypeEntity? RecipeType { get; private set; }
  public int? RecipeTypeId { get; private set; }

  public ContentStatus Status { get; private set; }
  public long? PublishedVersion { get; private set; }
  public string? PublishedBy { get; private set; }
  public DateTime? PublishedOn { get; private set; }

  public List<RecipeLocaleEntity> Locales { get; private set; } = [];

  public RecipeEntity(KitchenEntity kitchen, RecipeCreated @event) : base(@event)
  {
    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    EntityId = new RecipeId(@event.StreamId).EntityId;

    Name = @event.Name.Value;
  }

  private RecipeEntity() : base()
  {
  }

  public void Annotate(RecipeAnnotated @event)
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
    foreach (RecipeLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public void Publish(RecipePublished @event)
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
      RecipeLocaleEntity locale = FindLocale(@event.Language);
      locale.Publish(@event);
    }
  }

  public RecipeLocaleEntity? RemoveLocale(RecipeLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
  }

  public void Rename(RecipeRenamed @event)
  {
    UpdateInvariant(@event);

    Name = @event.Name.Value;
  }

  public void SetLocale(RecipeLocaleChanged @event)
  {
    base.Update(@event);

    RecipeLocaleEntity? locale = TryGetLocale(@event.Language);
    if (locale is null)
    {
      locale = new RecipeLocaleEntity(this, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }

  public void SetType(RecipeTypeEntity? recipeType, RecipeTyped @event)
  {
    base.Update(@event);

    RecipeType = recipeType;
    RecipeTypeId = recipeType?.RecipeTypeId;
  }

  public void Unpublish(RecipeUnpublished @event)
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
      RecipeLocaleEntity locale = FindLocale(@event.Language);
      locale.Unpublish(@event);
    }
  }

  private RecipeLocaleEntity FindLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The recipe type '{this}' locale '{language}' was not found.");
  private RecipeLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

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

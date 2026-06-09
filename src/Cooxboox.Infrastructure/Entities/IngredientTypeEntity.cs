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
  public string? Notes { get; private set; }

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

  public override IReadOnlyCollection<ActorId> GetActorIds()
  {
    HashSet<ActorId> actorIds = base.GetActorIds().ToHashSet();
    foreach (IngredientTypeLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }
    return actorIds;
  }

  public IngredientTypeLocaleEntity? RemoveLocale(IngredientTypeLocaleRemoved @event)
  {
    base.Update(@event);

    return TryGetLocale(@event.Language);
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

  public void Update(IngredientTypeUpdated @event)
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

  private IngredientTypeLocaleEntity? TryGetLocale(Language language) => Locales.SingleOrDefault(locale => locale.Language == language.Code);

  public override string ToString() => $"{Name} | {base.ToString()}";
}

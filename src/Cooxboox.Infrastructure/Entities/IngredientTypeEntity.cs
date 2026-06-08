using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Events;

namespace Cooxboox.Infrastructure.Entities;

internal class IngredientTypeEntity : AggregateEntity
{
  public int IngredientTypeId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public Guid EntityId { get; private set; }

  public string Name { get; private set; } = string.Empty;
  public string? Notes { get; private set; }

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

  public override string ToString() => $"{Name} | {base.ToString()}";
}

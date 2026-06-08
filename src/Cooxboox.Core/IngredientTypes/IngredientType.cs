using Cooxboox.Core.IngredientTypes.Events;
using Cooxboox.Core.Kitchens;
using Logitar.EventSourcing;

namespace Cooxboox.Core.IngredientTypes;

public class IngredientType : AggregateRoot, IEntityProvider
{
  public const string EntityKind = "IngredientType";

  public new IngredientTypeId Id => new(base.Id);
  public Entity Entity => new(EntityKind, Id.EntityId, Id.KitchenId);

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");
  public Notes? Notes { get; private set; }

  public IngredientType() : base()
  {
  }

  public IngredientType(Kitchen kitchen, Name name, ActorId? actorId = null)
    : this(IngredientTypeId.NewId(kitchen.Id), name, actorId)
  {
  }

  public IngredientType(IngredientTypeId ingredientTypeId, Name name, ActorId? actorId = null)
    : base(ingredientTypeId.StreamId)
  {
    Raise(new IngredientTypeCreated(name), actorId);
  }
  protected virtual void Handle(IngredientTypeCreated @event)
  {
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new IngredientTypeDeleted(), actorId);
    }
  }

  public void Update(Name name, Notes? notes, ActorId? actorId = null)
  {
    IngredientTypeUpdated @event = new(
      Name.Equals(name) ? null : name,
      Equals(Notes, notes) ? null : new Optional<Notes>(notes));

    if (@event.Name is not null || @event.Notes is not null)
    {
      Raise(@event, actorId);
    }
  }
  protected virtual void Handle(IngredientTypeUpdated @event)
  {
    if (@event.Name is not null)
    {
      _name = @event.Name;
    }
    if (@event.Notes is not null)
    {
      Notes = @event.Notes.Value;
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

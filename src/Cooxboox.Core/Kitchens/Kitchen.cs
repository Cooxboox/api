using Cooxboox.Core.Kitchens.Events;
using Cooxboox.Core.Validation;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Kitchens;

public class Kitchen : AggregateRoot
{
  public new KitchenId Id => new(base.Id);
  public Guid EntityId => Id.ToGuid();

  public UserId OwnerId { get; private set; }

  public Confidentiality Confidentiality { get; private set; }

  private Name? _name = null;
  public Name Name => _name ?? throw new InvalidOperationException("The name was not initialized.");

  // TODO(fpion): Localization /w Publishing
  // TODO(fpion): Slug
  // TODO(fpion): MetaDescription
  // TODO(fpion): HtmlContent

  public Kitchen() : base()
  {
  }

  public Kitchen(UserId ownerId, Name name, Confidentiality confidentiality = default, KitchenId? kitchenId = null)
    : base((kitchenId ?? KitchenId.NewId()).StreamId)
  {
    Raise(new KitchenCreated(ownerId, confidentiality, name), ownerId.ActorId);
  }
  protected virtual void Handle(KitchenCreated @event)
  {
    OwnerId = @event.OwnerId;
    Confidentiality = @event.Confidentiality;
    _name = @event.Name;
  }

  public void Delete(ActorId? actorId = null)
  {
    if (!IsDeleted)
    {
      Raise(new KitchenDeleted(), actorId);
    }
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}

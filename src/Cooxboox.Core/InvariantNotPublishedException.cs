using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core;

public class InvariantNotPublishedException : DomainException
{
  private const string ErrorMessage = "The entity invariant is not published.";

  public Guid? KitchenId
  {
    get => (Guid?)Data[nameof(KitchenId)];
    private set => Data[nameof(KitchenId)] = value;
  }
  public string EntityKind
  {
    get => (string)Data[nameof(EntityKind)]!;
    private set => Data[nameof(EntityKind)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(KitchenId)] = KitchenId;
      error.Data[nameof(EntityKind)] = EntityKind;
      error.Data[nameof(EntityId)] = EntityId;
      return error;
    }
  }

  public InvariantNotPublishedException(IEntityProvider provider)
    : base(BuildMessage(provider))
  {
    Entity entity = provider.Entity;
    KitchenId = entity.KitchenId?.EntityId;
    EntityKind = entity.Kind;
    EntityId = entity.Id;
  }

  private static string BuildMessage(IEntityProvider provider)
  {
    Entity entity = provider.Entity;
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(KitchenId), entity.KitchenId?.EntityId, "<null>")
      .AddData(nameof(EntityKind), entity.Kind)
      .AddData(nameof(EntityId), entity.Id)
      .Build();
  }
}

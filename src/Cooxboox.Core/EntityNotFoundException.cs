using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core;

public class EntityNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified entity was not found.";

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
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(KitchenId)] = KitchenId;
      error.Data[nameof(EntityKind)] = EntityKind;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public EntityNotFoundException(Entity entity, string propertyName)
    : base(BuildMessage(entity, propertyName))
  {
    KitchenId = entity.KitchenId?.EntityId;
    EntityKind = entity.Kind;
    EntityId = entity.Id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Entity entity, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), entity.KitchenId?.EntityId, "<null>")
    .AddData(nameof(EntityKind), entity.Kind)
    .AddData(nameof(EntityId), entity.Id)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

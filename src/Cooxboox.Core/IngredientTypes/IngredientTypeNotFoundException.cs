using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.IngredientTypes;

public class IngredientTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified ingredient type was not found.";

  public Guid KitchenId
  {
    get => (Guid)Data[nameof(KitchenId)]!;
    private set => Data[nameof(KitchenId)] = value;
  }
  public Guid IngredientTypeId
  {
    get => (Guid)Data[nameof(IngredientTypeId)]!;
    private set => Data[nameof(IngredientTypeId)] = value;
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
      error.Data[nameof(IngredientTypeId)] = IngredientTypeId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public IngredientTypeNotFoundException(IngredientTypeId ingredientTypeId, string propertyName)
    : base(BuildMessage(ingredientTypeId, propertyName))
  {
    KitchenId = ingredientTypeId.KitchenId.EntityId;
    IngredientTypeId = ingredientTypeId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(IngredientTypeId ingredientTypeId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), ingredientTypeId.KitchenId.EntityId)
    .AddData(nameof(IngredientTypeId), ingredientTypeId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

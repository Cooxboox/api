using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.RecipeTypes;

public class RecipeTypeNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified recipe type was not found.";

  public Guid KitchenId
  {
    get => (Guid)Data[nameof(KitchenId)]!;
    private set => Data[nameof(KitchenId)] = value;
  }
  public Guid RecipeTypeId
  {
    get => (Guid)Data[nameof(RecipeTypeId)]!;
    private set => Data[nameof(RecipeTypeId)] = value;
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
      error.Data[nameof(RecipeTypeId)] = RecipeTypeId;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RecipeTypeNotFoundException(RecipeTypeId recipeTypeId, string propertyName)
    : base(BuildMessage(recipeTypeId, propertyName))
  {
    KitchenId = recipeTypeId.KitchenId.EntityId;
    RecipeTypeId = recipeTypeId.EntityId;
    PropertyName = propertyName;
  }

  private static string BuildMessage(RecipeTypeId recipeTypeId, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), recipeTypeId.KitchenId.EntityId)
    .AddData(nameof(RecipeTypeId), recipeTypeId.EntityId)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}

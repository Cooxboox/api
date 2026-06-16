using Logitar;

namespace Cooxboox.Core;

public class KitchenMismatchException : Exception
{
  private const string ErrorMessage = "The specified entities must reside in the same kitchen.";

  public string ExpectedEntity
  {
    get => (string)Data[nameof(ExpectedEntity)]!;
    private set => Data[nameof(ExpectedEntity)] = value;
  }
  public string AttemptedEntity
  {
    get => (string)Data[nameof(AttemptedEntity)]!;
    private set => Data[nameof(AttemptedEntity)] = value;
  }

  public KitchenMismatchException(Entity expectedEntity, Entity attemptedEntity)
    : base(BuildMessage(expectedEntity, attemptedEntity))
  {
    ExpectedEntity = expectedEntity.ToString();
    AttemptedEntity = attemptedEntity.ToString();
  }

  private static string BuildMessage(Entity expectedEntity, Entity attemptedEntity) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ExpectedEntity), expectedEntity)
    .AddData(nameof(AttemptedEntity), attemptedEntity)
    .Build();

  public static void ThrowIfMismatch(Entity expectedEntity, Entity attemptedEntity)
  {
    if (expectedEntity.KitchenId != attemptedEntity.KitchenId)
    {
      throw new KitchenMismatchException(expectedEntity, attemptedEntity);
    }
  }
}

using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.Kitchens;

public class KitchenSlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified slug is already used.";

  public Guid KitchenId
  {
    get => (Guid)Data[nameof(KitchenId)]!;
    private set => Data[nameof(KitchenId)] = value;
  }
  public Guid ConflictingId
  {
    get => (Guid)Data[nameof(ConflictingId)]!;
    private set => Data[nameof(ConflictingId)] = value;
  }
  public string AttemptedSlug
  {
    get => (string)Data[nameof(AttemptedSlug)]!;
    private set => Data[nameof(AttemptedSlug)] = value;
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
      error.Data[nameof(ConflictingId)] = ConflictingId;
      error.Data[nameof(AttemptedSlug)] = AttemptedSlug;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public KitchenSlugAlreadyUsedException(Kitchen kitchen, Guid conflictingId)
    : base(BuildMessage(kitchen, conflictingId))
  {
    KitchenId = kitchen.EntityId;
    ConflictingId = conflictingId;
    AttemptedSlug = kitchen.Slug ?? throw new ArgumentException("The slug is required.", nameof(kitchen));
    PropertyName = nameof(Kitchen.Slug);
  }

  private static string BuildMessage(Kitchen kitchen, Guid conflictingId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), kitchen.EntityId)
    .AddData(nameof(ConflictingId), conflictingId)
    .AddData(nameof(AttemptedSlug), kitchen.Slug)
    .AddData(nameof(PropertyName), nameof(Kitchen.Slug))
    .Build();
}

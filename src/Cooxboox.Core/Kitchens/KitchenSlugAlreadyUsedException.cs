using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.Kitchens;

public class KitchenSlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified slug is already used by another kitchen.";

  public Guid KitchenId
  {
    get => (Guid)Data[nameof(KitchenId)]!;
    private set => Data[nameof(KitchenId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
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
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(AttemptedSlug)] = AttemptedSlug;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public KitchenSlugAlreadyUsedException(Kitchen kitchen, Guid conflictId)
    : base(BuildMessage(kitchen, conflictId))
  {
    KitchenId = kitchen.Id;
    ConflictId = conflictId;
    AttemptedSlug = kitchen.Slug!; // TODO(fpion): implement
    PropertyName = nameof(Kitchen.Slug);
  }

  private static string BuildMessage(Kitchen kitchen, Guid conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), kitchen.Id)
    .AddData(nameof(ConflictId), conflictId)
    .AddData(nameof(AttemptedSlug), kitchen.Slug) // TODO(fpion): implement
    .AddData(nameof(PropertyName), nameof(Kitchen.Slug))
    .Build();
}

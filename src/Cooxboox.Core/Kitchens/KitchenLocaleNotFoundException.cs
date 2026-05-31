using Cooxboox.Core.Localization;
using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.Kitchens;

public class KitchenLocaleNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified kitchen locale was not found.";

  public Guid KitchenId
  {
    get => (Guid)Data[nameof(KitchenId)]!;
    private set => Data[nameof(KitchenId)] = value;
  }
  public string Language
  {
    get => (string)Data[nameof(Language)]!;
    private set => Data[nameof(Language)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(KitchenId)] = KitchenId;
      error.Data[nameof(Language)] = Language;
      return error;
    }
  }

  public KitchenLocaleNotFoundException(Kitchen kitchen, Language language)
    : base(BuildMessage(kitchen, language))
  {
    KitchenId = kitchen.EntityId;
    Language = language.Code;
  }

  private static string BuildMessage(Kitchen kitchen, Language language) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(KitchenId), kitchen.EntityId)
    .AddData(nameof(Language), language)
    .Build();
}

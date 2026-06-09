using Krakenar.Contracts;
using Logitar;

namespace Cooxboox.Core.Localization;

public class LocaleNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified locale was not found.";

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
      error.Data[nameof(EntityKind)] = EntityKind;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(Language)] = Language;
      return error;
    }
  }

  public LocaleNotFoundException(IEntityProvider provider, Language language)
    : base(BuildMessage(provider, language))
  {
    Entity entity = provider.Entity;
    KitchenId = entity.KitchenId?.EntityId;
    EntityKind = entity.Kind;
    EntityId = entity.Id;
    Language = language.ToString();
  }

  private static string BuildMessage(IEntityProvider provider, Language language)
  {
    Entity entity = provider.Entity;
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(KitchenId), entity.KitchenId?.EntityId, "<null>")
      .AddData(nameof(EntityKind), entity.Kind)
      .AddData(nameof(EntityId), entity.Id)
      .AddData(nameof(Language), language)
      .Build();
  }
}

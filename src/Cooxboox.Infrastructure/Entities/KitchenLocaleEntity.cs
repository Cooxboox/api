using Cooxboox.Core.Kitchens.Events;

namespace Cooxboox.Infrastructure.Entities;

internal class KitchenLocaleEntity
{
  public int KitchenLocaleId { get; private set; }
  public Guid UniqueId { get; private set; }

  public KitchenEntity? Kitchen { get; private set; }
  public int KitchenId { get; private set; }
  public string Language { get; private set; } = string.Empty;

  public string? HtmlContent { get; private set; }
  public string? MetaDescription { get; private set; }

  public KitchenLocaleEntity(KitchenEntity kitchen, KitchenLocaleChanged @event)
  {
    UniqueId = Guid.NewGuid();

    Kitchen = kitchen;
    KitchenId = kitchen.KitchenId;
    Language = @event.Language.Code;

    Update(@event);
  }

  private KitchenLocaleEntity()
  {
  }

  public void Update(KitchenLocaleChanged @event)
  {
    HtmlContent = @event.Locale.HtmlContent?.Value;
    MetaDescription = @event.Locale.MetaDescription?.Value;
  }
}

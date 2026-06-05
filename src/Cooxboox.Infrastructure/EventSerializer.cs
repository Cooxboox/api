using Cooxboox.Infrastructure.Converters;

namespace Cooxboox.Infrastructure;

internal class EventSerializer : Logitar.EventSourcing.Infrastructure.EventSerializer
{
  protected override void RegisterConverters()
  {
    base.RegisterConverters();

    SerializerOptions.Converters.Add(new KitchenIdConverter());
    SerializerOptions.Converters.Add(new NameConverter());
    SerializerOptions.Converters.Add(new UserIdConverter());
  }
}

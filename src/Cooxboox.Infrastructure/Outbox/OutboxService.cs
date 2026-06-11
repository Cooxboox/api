using Cooxboox.Infrastructure.Entities;
using Krakenar.Contracts;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace Cooxboox.Infrastructure.Outbox;

internal interface IOutboxService
{
  Task HandleAsync<T>(T @event, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : DomainEvent;
}

internal class OutboxService : IOutboxService
{
  private readonly IDbContextFactory<CooxbooxContext> _factory;
  private readonly ILogger<OutboxService> _logger;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public OutboxService(IDbContextFactory<CooxbooxContext> factory, ILogger<OutboxService> logger)
  {
    _factory = factory;
    _logger = logger;
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public async Task HandleAsync<T>(T @event, Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken) where T : DomainEvent
  {
    OutboxMessageEntity message = new(@event);
    ExceptionDispatchInfo? handlerException = null;
    try
    {
      await handler(@event, cancellationToken);
    }
    catch (Exception exception)
    {
      handlerException = ExceptionDispatchInfo.Capture(exception);

      Error error = new(exception);
      string json = JsonSerializer.Serialize(error, _serializerOptions);
      message.Fail(json);
    }

    try
    {
      await using CooxbooxContext context = await _factory.CreateDbContextAsync(CancellationToken.None);
      context.OutboxMessages.Add(message);
      await context.SaveChangesAsync(CancellationToken.None);
    }
    catch (Exception outboxException)
    {
      if (handlerException is not null)
      {
        _logger.LogError(handlerException.SourceException, "Failed to handle event '{EventType}' (Id={EventId}).", @event.GetType(), @event.Id);
      }
      _logger.LogError(outboxException, "Failed to persist outbox message for event '{EventType}' (Id={EventId}).", @event.GetType(), @event.Id);
    }
  }
}

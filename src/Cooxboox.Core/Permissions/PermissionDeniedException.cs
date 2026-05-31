using Krakenar.Contracts;
using Logitar;
using Logitar.EventSourcing;

namespace Cooxboox.Core.Permissions;

public class PermissionDeniedException : ErrorException
{
  private const string ErrorMessage = "The specified permission was denied.";

  public string? ActorId
  {
    get => (string?)Data[nameof(ActorId)];
    private set => Data[nameof(ActorId)] = value;
  }
  public string Action
  {
    get => (string)Data[nameof(Action)]!;
    private set => Data[nameof(Action)] = value;
  }
  public string? Resource
  {
    get => (string?)Data[nameof(Resource)];
    private set => Data[nameof(Resource)] = value;
  }

  public override Error Error => new(this.GetErrorCode(), ErrorMessage);

  public PermissionDeniedException(string action, ActorId? actorId = null, Entity? resource = null)
    : base(BuildMessage(action, actorId, resource))
  {
    ActorId = actorId?.Value;
    Action = action;
    Resource = resource?.ToString();
  }

  private static string BuildMessage(string action, ActorId? actorId, Entity? resource) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(ActorId), actorId, "<null>")
    .AddData(nameof(Action), action)
    .AddData(nameof(Resource), resource, "<null>")
    .Build();
}

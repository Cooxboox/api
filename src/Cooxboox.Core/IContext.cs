namespace Cooxboox.Core;

public interface IContext
{
  Guid UserId { get; }

  Guid? TryGetUserId();
}

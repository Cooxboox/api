namespace Cooxboox.Core;

public interface IContext
{
  Guid UserId { get; }

  bool IsKitchenOwner { get; }

  Guid? TryGetKitchenId();
  Guid? TryGetUserId();

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

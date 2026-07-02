using Krakenar.Contracts;

namespace Cooxboox.Core;

public interface IContext
{
  Guid KitchenId { get; }
  Guid UserId { get; }

  bool IsKitchenOwner { get; }

  IReadOnlyCollection<CustomAttribute> GetSessionCustomAttributes();

  Guid? TryGetKitchenId();
  Guid? TryGetUserId();

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

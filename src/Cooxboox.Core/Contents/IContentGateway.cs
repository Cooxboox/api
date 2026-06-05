using Krakenar.Contracts.Contents;

namespace Cooxboox.Core.Contents;

public interface IContentGateway
{
  Task<Content> CreateAsync(Guid contentTypeId, string uniqueName, CreateContentOptions? options = null, CancellationToken cancellationToken = default);
  Task<Content?> FindAsync(Guid contentTypeId, string uniqueName, CancellationToken cancellationToken = default);
  Task<Content> SaveLocaleAsync(Guid id, string uniqueName, SaveContentLocaleOptions? options = null, CancellationToken cancellationToken = default);
  Task<Content> UpdateLocaleAsync(Guid id, UpdateContentLocaleOptions? options = null, CancellationToken cancellationToken = default);
}

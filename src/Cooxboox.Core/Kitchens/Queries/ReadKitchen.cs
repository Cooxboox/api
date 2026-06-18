using Cooxboox.Core.Kitchens.Models;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;

namespace Cooxboox.Core.Kitchens.Queries;

internal record ReadKitchenQuery(Guid Id) : IQuery<KitchenModel?>;

internal class ReadKitchenQueryHandler : IQueryHandler<ReadKitchenQuery, KitchenModel?>
{
  private readonly IContext _context;
  private readonly DbSet<Kitchen> _kitchens;

  public ReadKitchenQueryHandler(IContext context, IDbContext database)
  {
    _context = context;
    _kitchens = database.Kitchens;
  }

  public async Task<KitchenModel?> HandleAsync(ReadKitchenQuery query, CancellationToken cancellationToken)
  {
    Kitchen? kitchen = await _kitchens.AsNoTracking()
      .Where(x => x.EntityId == query.Id && x.OwnerId == _context.UserId)
      .SingleOrDefaultAsync(cancellationToken);

    return null; // TODO(fpion): map
  }
}

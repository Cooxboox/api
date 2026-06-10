using Cooxboox.Core.RecipeCategories.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeCategories.Queries;

internal record ReadRecipeCategoryQuery(Guid Id) : IQuery<RecipeCategoryModel?>;

internal class ReadRecipeCategoryQueryHandler : IQueryHandler<ReadRecipeCategoryQuery, RecipeCategoryModel?>
{
  private readonly IRecipeCategoryQuerier _recipeCategoryQuerier;

  public ReadRecipeCategoryQueryHandler(IRecipeCategoryQuerier recipeCategoryQuerier)
  {
    _recipeCategoryQuerier = recipeCategoryQuerier;
  }

  public async Task<RecipeCategoryModel?> HandleAsync(ReadRecipeCategoryQuery query, CancellationToken cancellationToken)
  {
    return await _recipeCategoryQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

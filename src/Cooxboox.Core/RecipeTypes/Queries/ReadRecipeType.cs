using Cooxboox.Core.RecipeTypes.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.RecipeTypes.Queries;

internal record ReadRecipeTypeQuery(Guid Id) : IQuery<RecipeTypeModel?>;

internal class ReadRecipeTypeQueryHandler : IQueryHandler<ReadRecipeTypeQuery, RecipeTypeModel?>
{
  private readonly IRecipeTypeQuerier _recipeTypeQuerier;

  public ReadRecipeTypeQueryHandler(IRecipeTypeQuerier recipeTypeQuerier)
  {
    _recipeTypeQuerier = recipeTypeQuerier;
  }

  public async Task<RecipeTypeModel?> HandleAsync(ReadRecipeTypeQuery query, CancellationToken cancellationToken)
  {
    return await _recipeTypeQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

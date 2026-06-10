using Cooxboox.Core.Recipes.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.Recipes.Queries;

internal record ReadRecipeQuery(Guid Id) : IQuery<RecipeModel?>;

internal class ReadRecipeQueryHandler : IQueryHandler<ReadRecipeQuery, RecipeModel?>
{
  private readonly IRecipeQuerier _recipeQuerier;

  public ReadRecipeQueryHandler(IRecipeQuerier recipeQuerier)
  {
    _recipeQuerier = recipeQuerier;
  }

  public async Task<RecipeModel?> HandleAsync(ReadRecipeQuery query, CancellationToken cancellationToken)
  {
    return await _recipeQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

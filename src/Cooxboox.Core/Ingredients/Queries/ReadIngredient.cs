using Cooxboox.Core.Ingredients.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.Ingredients.Queries;

internal record ReadIngredientQuery(Guid Id) : IQuery<IngredientModel?>;

internal class ReadIngredientQueryHandler : IQueryHandler<ReadIngredientQuery, IngredientModel?>
{
  private readonly IIngredientQuerier _ingredientQuerier;

  public ReadIngredientQueryHandler(IIngredientQuerier ingredientQuerier)
  {
    _ingredientQuerier = ingredientQuerier;
  }

  public async Task<IngredientModel?> HandleAsync(ReadIngredientQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

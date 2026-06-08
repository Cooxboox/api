using Cooxboox.Core.IngredientTypes.Models;
using Logitar.CQRS;

namespace Cooxboox.Core.IngredientTypes.Queries;

internal record ReadIngredientTypeQuery(Guid Id) : IQuery<IngredientTypeModel?>;

internal class ReadIngredientTypeQueryHandler : IQueryHandler<ReadIngredientTypeQuery, IngredientTypeModel?>
{
  private readonly IIngredientTypeQuerier _ingredientTypeQuerier;

  public ReadIngredientTypeQueryHandler(IIngredientTypeQuerier ingredientTypeQuerier)
  {
    _ingredientTypeQuerier = ingredientTypeQuerier;
  }

  public async Task<IngredientTypeModel?> HandleAsync(ReadIngredientTypeQuery query, CancellationToken cancellationToken)
  {
    return await _ingredientTypeQuerier.ReadAsync(query.Id, cancellationToken);
  }
}

using Cooxboox.Core.Ingredients.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.Ingredients;

public interface IIngredientQuerier
{
  Task<IngredientModel> ReadAsync(Ingredient ingredient, CancellationToken cancellationToken = default);
  Task<IngredientModel?> ReadAsync(IngredientId id, CancellationToken cancellationToken = default);
  Task<IngredientModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<IngredientModel>> SearchAsync(SearchIngredientsPayload payload, CancellationToken cancellationToken = default);
}

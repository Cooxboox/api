using Cooxboox.Core.IngredientTypes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.IngredientTypes;

public interface IIngredientTypeQuerier
{
  Task<IngredientTypeModel> ReadAsync(IngredientType ingredientType, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> ReadAsync(IngredientTypeId id, CancellationToken cancellationToken = default);
  Task<IngredientTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<IngredientTypeModel>> SearchAsync(SearchIngredientTypesPayload payload, CancellationToken cancellationToken = default);
}

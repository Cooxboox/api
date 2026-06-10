using Cooxboox.Core.RecipeTypes.Models;
using Krakenar.Contracts.Search;

namespace Cooxboox.Core.RecipeTypes;

public interface IRecipeTypeQuerier
{
  Task<RecipeTypeModel> ReadAsync(RecipeType recipeType, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> ReadAsync(RecipeTypeId id, CancellationToken cancellationToken = default);
  Task<RecipeTypeModel?> ReadAsync(Guid id, CancellationToken cancellationToken = default);

  Task<SearchResults<RecipeTypeModel>> SearchAsync(SearchRecipeTypesPayload payload, CancellationToken cancellationToken = default);
}

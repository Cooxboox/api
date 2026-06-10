using Cooxboox.Core.Recipes;
using Cooxboox.Core.Recipes.Models;
using Cooxboox.Filters;
using Cooxboox.Models.Recipe;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("recipes")]
public class RecipeController : ControllerBase
{
  private readonly IRecipeService _recipeService;

  public RecipeController(IRecipeService recipeService)
  {
    _recipeService = recipeService;
  }

  [HttpPost]
  public async Task<ActionResult<RecipeModel>> CreateAsync([FromBody] CreateOrReplaceRecipePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeResult result = await _recipeService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<RecipeModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.PublishAllAsync(id, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<RecipeModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.PublishAsync(id, language, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RecipeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.ReadAsync(id, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RecipeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRecipePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeResult result = await _recipeService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveRecipeLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RecipeModel>>> SearchAsync([FromQuery] SearchRecipesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRecipesPayload payload = parameters.ToPayload();
    SearchResults<RecipeModel> recipes = await _recipeService.SearchAsync(payload, cancellationToken);
    return Ok(recipes);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<RecipeModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.UnpublishAllAsync(id, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<RecipeModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.UnpublishAsync(id, language, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RecipeModel>> UpdateAsync(Guid id, [FromBody] UpdateRecipePayload payload, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.UpdateAsync(id, payload, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateRecipeLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeModel? recipe = await _recipeService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return recipe is null ? NotFound() : Ok(recipe);
  }

  private ActionResult<RecipeModel> ToActionResult(CreateOrReplaceRecipeResult result)
  {
    RecipeModel recipe = result.Recipe;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/recipes/{recipe.Id}", UriKind.Absolute);
      return Created(location, recipe);
    }
    return Ok(recipe);
  }
}

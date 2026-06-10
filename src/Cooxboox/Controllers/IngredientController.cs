using Cooxboox.Core.Ingredients;
using Cooxboox.Core.Ingredients.Models;
using Cooxboox.Filters;
using Cooxboox.Models.Ingredient;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("ingredients")]
public class IngredientController : ControllerBase
{
  private readonly IIngredientService _ingredientService;

  public IngredientController(IIngredientService ingredientService)
  {
    _ingredientService = ingredientService;
  }

  [HttpPost]
  public async Task<ActionResult<IngredientModel>> CreateAsync([FromBody] CreateOrReplaceIngredientPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientResult result = await _ingredientService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<IngredientModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.PublishAllAsync(id, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<IngredientModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.PublishAsync(id, language, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<IngredientModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.ReadAsync(id, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<IngredientModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceIngredientPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientResult result = await _ingredientService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveIngredientLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<IngredientModel>>> SearchAsync([FromQuery] SearchIngredientsParameters parameters, CancellationToken cancellationToken)
  {
    SearchIngredientsPayload payload = parameters.ToPayload();
    SearchResults<IngredientModel> ingredients = await _ingredientService.SearchAsync(payload, cancellationToken);
    return Ok(ingredients);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<IngredientModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.UnpublishAllAsync(id, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<IngredientModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.UnpublishAsync(id, language, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<IngredientModel>> UpdateAsync(Guid id, [FromBody] UpdateIngredientPayload payload, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.UpdateAsync(id, payload, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateIngredientLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientModel? ingredient = await _ingredientService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return ingredient is null ? NotFound() : Ok(ingredient);
  }

  private ActionResult<IngredientModel> ToActionResult(CreateOrReplaceIngredientResult result)
  {
    IngredientModel ingredient = result.Ingredient;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/ingredients/{ingredient.Id}", UriKind.Absolute);
      return Created(location, ingredient);
    }
    return Ok(ingredient);
  }
}

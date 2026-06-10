using Cooxboox.Core.IngredientCategories;
using Cooxboox.Core.IngredientCategories.Models;
using Cooxboox.Filters;
using Cooxboox.Models.IngredientCategory;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("ingredients/categories")]
public class IngredientCategoryController : ControllerBase
{
  private readonly IIngredientCategoryService _ingredientCategoryService;

  public IngredientCategoryController(IIngredientCategoryService ingredientCategoryService)
  {
    _ingredientCategoryService = ingredientCategoryService;
  }

  [HttpPost]
  public async Task<ActionResult<IngredientCategoryModel>> CreateAsync([FromBody] CreateOrReplaceIngredientCategoryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientCategoryResult result = await _ingredientCategoryService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<IngredientCategoryModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.PublishAllAsync(id, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<IngredientCategoryModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.PublishAsync(id, language, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<IngredientCategoryModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.ReadAsync(id, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<IngredientCategoryModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceIngredientCategoryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientCategoryResult result = await _ingredientCategoryService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientCategoryModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveIngredientCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<IngredientCategoryModel>>> SearchAsync([FromQuery] SearchIngredientCategoriesParameters parameters, CancellationToken cancellationToken)
  {
    SearchIngredientCategoriesPayload payload = parameters.ToPayload();
    SearchResults<IngredientCategoryModel> ingredientCategories = await _ingredientCategoryService.SearchAsync(payload, cancellationToken);
    return Ok(ingredientCategories);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<IngredientCategoryModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UnpublishAllAsync(id, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<IngredientCategoryModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UnpublishAsync(id, language, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<IngredientCategoryModel>> UpdateAsync(Guid id, [FromBody] UpdateIngredientCategoryPayload payload, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UpdateAsync(id, payload, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientCategoryModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateIngredientCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientCategoryModel? ingredientCategory = await _ingredientCategoryService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return ingredientCategory is null ? NotFound() : Ok(ingredientCategory);
  }

  private ActionResult<IngredientCategoryModel> ToActionResult(CreateOrReplaceIngredientCategoryResult result)
  {
    IngredientCategoryModel ingredientCategory = result.IngredientCategory;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/ingredients/categories/{ingredientCategory.Id}", UriKind.Absolute);
      return Created(location, ingredientCategory);
    }
    return Ok(ingredientCategory);
  }
}

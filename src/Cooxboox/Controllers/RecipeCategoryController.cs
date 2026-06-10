using Cooxboox.Core.RecipeCategories;
using Cooxboox.Core.RecipeCategories.Models;
using Cooxboox.Filters;
using Cooxboox.Models.RecipeCategory;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("recipes/categories")]
public class RecipeCategoryController : ControllerBase
{
  private readonly IRecipeCategoryService _recipeCategoryService;

  public RecipeCategoryController(IRecipeCategoryService recipeCategoryService)
  {
    _recipeCategoryService = recipeCategoryService;
  }

  [HttpPost]
  public async Task<ActionResult<RecipeCategoryModel>> CreateAsync([FromBody] CreateOrReplaceRecipeCategoryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeCategoryResult result = await _recipeCategoryService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<RecipeCategoryModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.PublishAllAsync(id, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<RecipeCategoryModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.PublishAsync(id, language, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RecipeCategoryModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.ReadAsync(id, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RecipeCategoryModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRecipeCategoryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeCategoryResult result = await _recipeCategoryService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeCategoryModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveRecipeCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RecipeCategoryModel>>> SearchAsync([FromQuery] SearchRecipeCategoriesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRecipeCategoriesPayload payload = parameters.ToPayload();
    SearchResults<RecipeCategoryModel> recipeCategories = await _recipeCategoryService.SearchAsync(payload, cancellationToken);
    return Ok(recipeCategories);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<RecipeCategoryModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UnpublishAllAsync(id, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<RecipeCategoryModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UnpublishAsync(id, language, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RecipeCategoryModel>> UpdateAsync(Guid id, [FromBody] UpdateRecipeCategoryPayload payload, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UpdateAsync(id, payload, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeCategoryModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateRecipeCategoryLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeCategoryModel? recipeCategory = await _recipeCategoryService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return recipeCategory is null ? NotFound() : Ok(recipeCategory);
  }

  private ActionResult<RecipeCategoryModel> ToActionResult(CreateOrReplaceRecipeCategoryResult result)
  {
    RecipeCategoryModel recipeCategory = result.RecipeCategory;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/recipes/categories/{recipeCategory.Id}", UriKind.Absolute);
      return Created(location, recipeCategory);
    }
    return Ok(recipeCategory);
  }
}

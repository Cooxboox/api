using Cooxboox.Core.RecipeTypes;
using Cooxboox.Core.RecipeTypes.Models;
using Cooxboox.Filters;
using Cooxboox.Models.RecipeType;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("recipes/types")]
public class RecipeTypeController : ControllerBase
{
  private readonly IRecipeTypeService _recipeTypeService;

  public RecipeTypeController(IRecipeTypeService recipeTypeService)
  {
    _recipeTypeService = recipeTypeService;
  }

  [HttpPost]
  public async Task<ActionResult<RecipeTypeModel>> CreateAsync([FromBody] CreateOrReplaceRecipeTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeTypeResult result = await _recipeTypeService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<RecipeTypeModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.PublishAllAsync(id, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<RecipeTypeModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.PublishAsync(id, language, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<RecipeTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.ReadAsync(id, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<RecipeTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceRecipeTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceRecipeTypeResult result = await _recipeTypeService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeTypeModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveRecipeTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<RecipeTypeModel>>> SearchAsync([FromQuery] SearchRecipeTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchRecipeTypesPayload payload = parameters.ToPayload();
    SearchResults<RecipeTypeModel> recipeTypes = await _recipeTypeService.SearchAsync(payload, cancellationToken);
    return Ok(recipeTypes);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<RecipeTypeModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.UnpublishAllAsync(id, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<RecipeTypeModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.UnpublishAsync(id, language, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<RecipeTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateRecipeTypePayload payload, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.UpdateAsync(id, payload, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<RecipeTypeModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateRecipeTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    RecipeTypeModel? recipeType = await _recipeTypeService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return recipeType is null ? NotFound() : Ok(recipeType);
  }

  private ActionResult<RecipeTypeModel> ToActionResult(CreateOrReplaceRecipeTypeResult result)
  {
    RecipeTypeModel recipeType = result.RecipeType;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/recipes/types/{recipeType.Id}", UriKind.Absolute);
      return Created(location, recipeType);
    }
    return Ok(recipeType);
  }
}

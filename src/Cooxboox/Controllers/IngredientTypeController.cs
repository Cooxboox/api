using Cooxboox.Core.IngredientTypes;
using Cooxboox.Core.IngredientTypes.Models;
using Cooxboox.Filters;
using Cooxboox.Models.IngredientType;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[RequireKitchen]
[Route("ingredients/types")]
public class IngredientTypeController : ControllerBase
{
  private readonly IIngredientTypeService _ingredientTypeService;

  public IngredientTypeController(IIngredientTypeService ingredientTypeService)
  {
    _ingredientTypeService = ingredientTypeService;
  }

  [HttpPost]
  public async Task<ActionResult<IngredientTypeModel>> CreateAsync([FromBody] CreateOrReplaceIngredientTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientTypeResult result = await _ingredientTypeService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<IngredientTypeModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.PublishAllAsync(id, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<IngredientTypeModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.PublishAsync(id, language, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<IngredientTypeModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.ReadAsync(id, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<IngredientTypeModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceIngredientTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceIngredientTypeResult result = await _ingredientTypeService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientTypeModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveIngredientTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<IngredientTypeModel>>> SearchAsync([FromQuery] SearchIngredientTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchIngredientTypesPayload payload = parameters.ToPayload();
    SearchResults<IngredientTypeModel> ingredientTypes = await _ingredientTypeService.SearchAsync(payload, cancellationToken);
    return Ok(ingredientTypes);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<IngredientTypeModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.UnpublishAllAsync(id, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<IngredientTypeModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.UnpublishAsync(id, language, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<IngredientTypeModel>> UpdateAsync(Guid id, [FromBody] UpdateIngredientTypePayload payload, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.UpdateAsync(id, payload, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<IngredientTypeModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateIngredientTypeLocalePayload payload, CancellationToken cancellationToken)
  {
    IngredientTypeModel? ingredientType = await _ingredientTypeService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return ingredientType is null ? NotFound() : Ok(ingredientType);
  }

  private ActionResult<IngredientTypeModel> ToActionResult(CreateOrReplaceIngredientTypeResult result)
  {
    IngredientTypeModel ingredientType = result.IngredientType;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/ingredients/types/{ingredientType.Id}", UriKind.Absolute);
      return Created(location, ingredientType);
    }
    return Ok(ingredientType);
  }
}

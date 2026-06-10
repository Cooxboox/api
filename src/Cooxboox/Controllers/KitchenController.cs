using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Models.Kitchen;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
[Authorize]
[Route("kitchens")]
public class KitchenController : ControllerBase
{
  private readonly IKitchenService _kitchenService;

  public KitchenController(IKitchenService kitchenService)
  {
    _kitchenService = kitchenService;
  }

  [HttpPost]
  public async Task<ActionResult<KitchenModel>> CreateAsync([FromBody] CreateOrReplaceKitchenPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}/publish/all")]
  public async Task<ActionResult<KitchenModel>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.PublishAllAsync(id, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPatch("{id}/publish")]
  public async Task<ActionResult<KitchenModel>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.PublishAsync(id, language, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<KitchenModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.ReadAsync(id, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<KitchenModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceKitchenPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<KitchenModel>>> SearchAsync([FromQuery] SearchKitchensParameters parameters, CancellationToken cancellationToken)
  {
    SearchKitchensPayload payload = parameters.ToPayload();
    SearchResults<KitchenModel> kitchens = await _kitchenService.SearchAsync(payload, cancellationToken);
    return Ok(kitchens);
  }

  [HttpPut("{id}/locales/{language}")]
  public async Task<ActionResult<KitchenModel>> SaveLocaleAsync(Guid id, string language, [FromBody] SaveKitchenLocalePayload payload, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.SaveLocaleAsync(id, language, payload, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPatch("{id}/unpublish/all")]
  public async Task<ActionResult<KitchenModel>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.UnpublishAllAsync(id, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPatch("{id}/unpublish")]
  public async Task<ActionResult<KitchenModel>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.UnpublishAsync(id, language, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<KitchenModel>> UpdateAsync(Guid id, [FromBody] UpdateKitchenPayload payload, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.UpdateAsync(id, payload, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  [HttpPatch("{id}/locales/{language}")]
  public async Task<ActionResult<KitchenModel>> UpdateLocaleAsync(Guid id, string language, [FromBody] UpdateKitchenLocalePayload payload, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.UpdateLocaleAsync(id, language, payload, cancellationToken);
    return kitchen is null ? NotFound() : Ok(kitchen);
  }

  private ActionResult<KitchenModel> ToActionResult(CreateOrReplaceKitchenResult result)
  {
    KitchenModel kitchen = result.Kitchen;
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/kitchens/{kitchen.Id}", UriKind.Absolute);
      return Created(location, kitchen);
    }
    return Ok(kitchen);
  }
}

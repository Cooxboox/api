using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
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
  public async Task<ActionResult<KitchenModel>> CreateAsync(string? language, [FromBody] CreateOrReplaceKitchenPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id: null, language, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<KitchenModel>> ReplaceAsync(Guid id, string? language, [FromBody] CreateOrReplaceKitchenPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceKitchenResult result = await _kitchenService.CreateOrReplaceAsync(payload, id, language, cancellationToken);
    return ToActionResult(result);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<KitchenModel>> UpdateAsync(Guid id, [FromBody] UpdateKitchenPayload payload, CancellationToken cancellationToken)
  {
    KitchenModel? kitchen = await _kitchenService.UpdateAsync(id, payload, cancellationToken);
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

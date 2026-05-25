using Cooxboox.Authentication;
using Cooxboox.Core.Identity;
using Cooxboox.Core.Identity.Models;
using Cooxboox.Extensions;
using Cooxboox.Models.Identity;
using Cooxboox.Settings;
using Krakenar.Client;
using Logitar;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cooxboox.Controllers;

[ApiController]
public class IdentityController : ControllerBase
{
  private readonly ErrorSettings _errorSettings;
  private readonly IIdentityService _identityService;
  private readonly ILogger<IdentityController> _logger;
  private readonly IOpenAuthenticationService _openAuthenticationService;

  public IdentityController(
    ErrorSettings errorSettings,
    IIdentityService identityService,
    ILogger<IdentityController> logger,
    IOpenAuthenticationService openAuthenticationService)
  {
    _errorSettings = errorSettings;
    _identityService = identityService;
    _logger = logger;
    _openAuthenticationService = openAuthenticationService;
  }

  [HttpPost("/auth/token")]
  public async Task<ActionResult<GetTokenResponse>> GetTokenAsync([FromBody] SignInAccountPayload payload, CancellationToken cancellationToken)
  {
    try
    {
      SignInAccountResult result = await _identityService.SignInAsync(payload, cancellationToken);

      GetTokenResponse response = new(result);
      if (result.Session is not null)
      {
        response.Token = await _openAuthenticationService.GetTokenResponseAsync(result.Session, cancellationToken);
      }
      return Ok(response);
    }
    catch (KrakenarClientException exception)
    {
      if (_errorSettings.ExposeDetail)
      {
        throw;
      }
      return InvalidCredentials(exception);
    }
  }

  [HttpPost("/sign/in")]
  public async Task<ActionResult<SignInAccountResponse>> SignInAsync([FromBody] SignInAccountRequest request, CancellationToken cancellationToken)
  {
    try
    {
      SignInAccountPayload payload = request.ToPayload();
      SignInAccountResult result = await _identityService.SignInAsync(payload, cancellationToken);
      if (result.Session is not null)
      {
        HttpContext.SignIn(result.Session);
      }

      SignInAccountResponse response = new(result);
      return Ok(response);
    }
    catch (KrakenarClientException exception)
    {
      if (_errorSettings.ExposeDetail)
      {
        throw;
      }
      return InvalidCredentials(exception);
    }
  }

  private ActionResult InvalidCredentials(KrakenarClientException exception)
  {
    string serializedError = JsonSerializer.Serialize(exception.Error);
    _logger.LogError(exception, "Invalid credentials: {Error}", serializedError);

    InvalidCredentialsError error = new();
    return Problem(
      detail: error.Message,
      instance: Request.GetDisplayUrl(),
      statusCode: StatusCodes.Status400BadRequest,
      title: error.Code.Humanize(),
      type: null,
      extensions: new Dictionary<string, object?> { ["error"] = error });
  }
}

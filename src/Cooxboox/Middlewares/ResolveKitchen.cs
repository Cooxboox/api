using Cooxboox.Constants;
using Cooxboox.Core.Kitchens;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Extensions;
using Krakenar.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Cooxboox.Middlewares;

internal class ResolveKitchen
{
  private readonly RequestDelegate _next;
  private readonly ProblemDetailsFactory _problemDetailsFactory;
  private readonly IProblemDetailsService _problemDetailsService;

  public ResolveKitchen(RequestDelegate next, ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
  {
    _next = next;
    _problemDetailsFactory = problemDetailsFactory;
    _problemDetailsService = problemDetailsService;
  }

  public async Task InvokeAsync(HttpContext context, IKitchenService kitchenService)
  {
    if (context.User.Identity is not null
        && context.User.Identity.IsAuthenticated
        && context.Request.Headers.TryGetValue(Headers.Kitchen, out StringValues values))
    {
      IReadOnlyCollection<string> sanitized = values.Sanitize();
      if (sanitized.Count > 1)
      {
        Error error = new(code: "InvalidKitchenHeader", message: "Only one kitchen header value is expected, but multiple were specified.");
        error.Data["Header"] = Headers.Kitchen;
        error.Data["SanitizedCount"] = sanitized.Count;
        error.Data["TotalCount"] = values.Count;
        await WriteResponseAsync(context, StatusCodes.Status400BadRequest, error);
        return;
      }
      else if (sanitized.Count == 1)
      {
        string value = sanitized.Single();
        bool parsed = Guid.TryParse(value, out Guid id);
        KitchenModel? kitchen = parsed ? await kitchenService.ReadAsync(id) : null;
        if (kitchen is null)
        {
          Error error = new(code: "KitchenNotFound", message: "The specified kitchen could not be found.");
          error.Data["Kitchen"] = value;
          error.Data["Header"] = Headers.Kitchen;
          await WriteResponseAsync(context, StatusCodes.Status404NotFound, error);
          return;
        }

        context.SetKitchen(kitchen);
      }
    }

    await _next(context);
  }

  private async Task WriteResponseAsync(HttpContext httpContext, int statusCode, Error error)
  {
    ProblemDetails problemDetails = _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, error);

    httpContext.Response.StatusCode = statusCode;
    ProblemDetailsContext context = new()
    {
      HttpContext = httpContext,
      ProblemDetails = problemDetails
    };
    _ = await _problemDetailsService.TryWriteAsync(context);
  }
}

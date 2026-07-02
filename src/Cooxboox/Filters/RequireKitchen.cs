using Cooxboox.Constants;
using Cooxboox.Core.Kitchens.Models;
using Cooxboox.Extensions;
using Krakenar.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Cooxboox.Filters;

internal class RequireKitchen : ActionFilterAttribute
{
  public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    HttpContext httpContext = context.HttpContext;
    KitchenModel? kitchen = httpContext.GetKitchen();
    if (kitchen is null)
    {
      Error error = new(code: "MissingKitchen", message: "A kitchen is required.");
      error.Data["Header"] = Headers.Kitchen;

      ProblemDetailsFactory problemDetailsFactory = httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
      ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, error);

      IProblemDetailsService problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
      ProblemDetailsContext problemDetailsContext = new()
      {
        HttpContext = httpContext,
        ProblemDetails = problemDetails
      };
      await problemDetailsService.WriteAsync(problemDetailsContext);
    }
    else
    {
      await next();
    }
  }
}

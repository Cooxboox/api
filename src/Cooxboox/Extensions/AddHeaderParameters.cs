using Cooxboox.Constants;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cooxboox.Extensions;

internal class AddHeaderParameters : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    operation.Parameters?.Add(new OpenApiParameter
    {
      In = ParameterLocation.Header,
      Name = Headers.Kitchen,
      Description = "Enter your kitchen ID in the input below:"
    });
  }
}

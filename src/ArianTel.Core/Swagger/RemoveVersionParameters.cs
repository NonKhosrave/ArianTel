using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArianTel.Core.Swagger;
public sealed class RemoveVersionParameters : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Remove version parameter from all Operations
        var versionParameter = operation.Parameters.SingleOrDefault(p => string.Equals(p.Name, "version", System.StringComparison.OrdinalIgnoreCase));
        if (versionParameter != null)
            operation.Parameters.Remove(versionParameter);
    }
}

using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArianTel.Core.Swagger;
public sealed class FormFileSwaggerFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formFileParams = context.MethodInfo.GetParameters()
            .Where(p => typeof(IFormFile).IsAssignableFrom(p.ParameterType))
            .Select(p => p.Name)
            .ToList();

        if (formFileParams.Any() && operation.RequestBody != null)
        {
            var content = operation.RequestBody.Content;

            foreach (var paramName in formFileParams)
            {
                if (content.TryGetValue("multipart/form-data", out var value))
                {
                    var schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                    value.Schema.Properties[paramName] = schema;
                }
            }
        }
    }
}

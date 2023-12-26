using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace BuildingBlocks.Model;
public sealed class ApiVersionErrorResponse : IErrorResponseProvider
{
    public IActionResult CreateResponse(ErrorResponseContext context)
    {
        return new ObjectResult(new
        {
            Error = new
            {
                Code = context.ErrorCode
            }
        })
        {
            StatusCode = 400
        };
    }
}

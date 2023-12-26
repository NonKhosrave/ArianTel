using System.Linq;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildingBlocks.Filter;
public sealed class ApiResultFilterAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var traceId = context.HttpContext.GenerateTraceId();
        if (context.Result is OkObjectResult okObjectResult)
        {
            var apiResult = new ApiResult<object>(okObjectResult.Value);
            context.Result = new JsonResult(apiResult) { StatusCode = okObjectResult.StatusCode };
        }
        else if (context.Result is BadRequestObjectResult badRequestObjectResult)
        {
            if (!context.ModelState.IsValid)
            {
                InitiateErrorModel(context, badRequestObjectResult, traceId);
            }
            else if (badRequestObjectResult.Value is ValidationError error)
            {
                var apiResult = new { Error = error.WithTraceIdentifier(traceId) };
                context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
            }
            else if (badRequestObjectResult.Value is ServiceResContextBase resContextBase)
            {
                var withTraceIdentifier = resContextBase.Error.WithTraceIdentifier(traceId);
                var apiResult = new { withTraceIdentifier };

                context.Result = new JsonResult(apiResult) { StatusCode = badRequestObjectResult.StatusCode };
            }
        }
        else if (context.Result is ObjectResult objectResult)
        {
            if (context.ModelState.IsValid)
            {
                if (objectResult.Value is ValidationError error)
                {
                    var apiResult = new { Error = error.WithTraceIdentifier(traceId) };
                    context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
                }
                else
                {
                    var apiResult = new ApiResult<object>(objectResult.Value);
                    context.Result = new JsonResult(apiResult) { StatusCode = objectResult.StatusCode };
                }
            }
            else
            {
                InitiateErrorModel(context, objectResult, traceId);
            }
        }

        base.OnResultExecuting(context);
    }

    private static void InitiateErrorModel(ResultExecutingContext context, ObjectResult objectResult, string traceId)
    {
        var modelState = context.ModelState;
        var errorDetailDtos = modelState.Keys
            .SelectMany(key => modelState[key].Errors.Select(x =>
                new ValidationError("InvalidModel", x.ErrorMessage, key)))
            .Distinct()
            .Where(r => !string.IsNullOrEmpty(r.Message))
            .ToList();

        var exceptionMessage = (context.ModelState.Values.FirstOrDefault()?
                                   .Errors?.FirstOrDefault()?.Exception?.Message ?? context.ModelState.Values
                                   .FirstOrDefault()?
                                   .Errors?.FirstOrDefault()?.ErrorMessage) ??
                               "One or more validation errors occurred.";

        var errorDto = new ErrorDto
        (
            new ValidationError
            (
                "InvalidModel",
                exceptionMessage,
                errorDetailDtos,
                traceId
            )
        );

        context.Result = new JsonResult(errorDto, Service.Extensions.JsonSerializerOptions)
        { StatusCode = objectResult.StatusCode };
    }
}

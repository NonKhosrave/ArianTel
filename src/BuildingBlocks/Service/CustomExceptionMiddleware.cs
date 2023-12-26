using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Logger;
using BuildingBlocks.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Service;
public sealed class CustomExceptionMiddleware
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CustomExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly bool _useStringEnumConverter;

    public CustomExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env,
        ILogger<CustomExceptionMiddleware> logger, IConfiguration configuration,
        bool useStringEnumConverter)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _env = env;
        _logger = logger;
        _configuration = configuration;
        _useStringEnumConverter = useStringEnumConverter;
    }

#pragma warning disable MA0051 // Method is too long
    public async Task InvokeAsync(HttpContext context)
#pragma warning restore MA0051 // Method is too long
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (CustomException e)
        {
            var traceId = context.GenerateTraceId();
            if (context.Response.HasStarted)
                throw;

            var error = new ErrorDto(e.Error.Error.WithTraceIdentifier(traceId));

            var json = error.ToJson(_useStringEnumConverter
                ? Extensions.JsonSerializerOptions
                : Extensions.JsonSerializerOptionsWithoutEnumString);

            context.Response.StatusCode = e.Error.Error.HttpStatusCode == 0
                ? 400
                : e.Error.Error.HttpStatusCode;

            context.Response.ContentType = "application/json";

            var errorHeaders = e.Error.Error.Headers;
            if (!string.IsNullOrEmpty(errorHeaders))
                try
                {
                    var list = errorHeaders.Split(';').ToList();

                    foreach (var h in list)
                    {
                        var split = h.Split('=');

                        context.Response.Headers.Add(split[0], split[1]);
                    }
                }
                catch
                {
                    context.Response.StatusCode = 500;

                    var errorDto = new ErrorDto
                    (
                        new ValidationError("ServerError",
                            $"invalid custom header: {errorHeaders}. correct header: f=10;h=9", new List<ValidationError>(), traceId)
                    );
                    json = errorDto.ToJson(_useStringEnumConverter
                        ? Extensions.JsonSerializerOptions
                        : Extensions.JsonSerializerOptionsWithoutEnumString);
                }

            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            var traceId = context.GenerateTraceId();
            _logger.CompileLog(exc, LogLevel.Error, exc.Message);

            if (context.Response.HasStarted)
                throw;

            var message = "Internal Server Error";
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("exception", "Internal Server Error");

            var details = new List<ValidationError>();

            var isShowFullError = _configuration.GetValue("ExceptionShowFullError", false);

            if (_env.IsDevelopment() || isShowFullError)
            {
                message = $"#Message: {exc.Message}, #StackTrace: {exc.StackTrace}";

                var exception = exc.InnerException;

                while (exception != null)
                {
                    //var customException = exception as CustomException;

                    details.Add(new ValidationError(
                        "ServerError",
                        $"#Message: {exception.Message} #StackTrace: {exception.StackTrace}"
                    ));

                    exception = exception.InnerException;
                }
            }

            var errorDto = new ErrorDto
            (
                new ValidationError
                (
                    "ServerError",
                    message,
                    details,
                    traceId
                )
            );

            var json = errorDto.ToJson(_useStringEnumConverter
                ? Extensions.JsonSerializerOptions
                : Extensions.JsonSerializerOptionsWithoutEnumString);
            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }
    }
}

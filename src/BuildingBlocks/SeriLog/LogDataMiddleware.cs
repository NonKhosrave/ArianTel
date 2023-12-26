using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BuildingBlocks.Logger;
using BuildingBlocks.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace BuildingBlocks.SeriLog;
public sealed class LogHeaderMiddleware
{
    private readonly EnvPackages _envPackages;
    private readonly ILogger<LogHeaderMiddleware> _logger;
    private readonly RequestDelegate _next;

    public LogHeaderMiddleware(RequestDelegate next, EnvPackages envPackages, ILogger<LogHeaderMiddleware> logger)
    {
        _next = next;
        _envPackages = envPackages;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var httpRequest = context.Request;
        try
        {
            var isExistHeader = httpRequest.Headers.TryGetValue("RequestId", out var value);
            var requestId = isExistHeader ? value.First() : Guid.NewGuid().ToString();

            context.Items["RequestId"] = requestId;
            var userIp = httpRequest.GetHeader<string>("clientip", false);
            var headers = httpRequest.Headers.Select(r => new { r.Key, r.Value }).Distinct()
                .ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
            headers.Remove("Authorization");
            var headerJson = headers.ToJson();

            LogContext.PushProperty("Application", _envPackages.AssemblyName());
            LogContext.PushProperty("HttpMethod", httpRequest.Method);
            LogContext.PushProperty("ServerRuntime", RuntimeInformation.FrameworkDescription);
            LogContext.PushProperty("Headers", headerJson);
            LogContext.PushProperty("CorrelationId", requestId);
            LogContext.PushProperty("UserIp", userIp);
            LogContext.PushProperty("Version", _envPackages.AssemblyVersion());
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }

        await _next(context).ConfigureAwait(false);

        LogContext.PushProperty("ResponseStatusCode", context.Response.StatusCode);
    }
}

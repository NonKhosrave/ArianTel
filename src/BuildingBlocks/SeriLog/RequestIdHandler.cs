using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Service;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.SeriLog;
public sealed class RequestIdHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestIdHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestId = _httpContextAccessor?.HttpContext?.Items["RequestId"]?.ToString() ??
                        _httpContextAccessor?.HttpContext?.Request?.GetHeader<string>("RequestId", false) ??
                        Guid.NewGuid().ToString();

        request.Headers.Add("RequestId", requestId);
        return base.SendAsync(request, cancellationToken);
    }
}

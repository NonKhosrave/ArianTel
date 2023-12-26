using System;
using BuildingBlocks.Service;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks;
public static class Extensions
{
    public static string GenerateTraceId(this HttpContext context)
    {
        return context.Request.GetHeader<string>("RequestId", false) ?? Guid.NewGuid().ToString();
    }
}

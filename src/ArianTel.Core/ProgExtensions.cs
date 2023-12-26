using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace ArianTel.Core;
public static class ProgExtensions
{
    public static int GetRandom(byte number)
    {
        var fromInclusive = (int)Math.Pow(10, number - 1);
        var exclusive = (int)Math.Pow(10, number);
        return RandomNumberGenerator.GetInt32(fromInclusive, exclusive);
    }

    public static string GetBaseUri(this HttpRequest request)
    {
        var newHost = string.Join('.', request.Host.Value.Split('.').Skip(1));
        newHost = string.IsNullOrEmpty(newHost) ? request.Host.Value : newHost;
        //var scheme = string.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase) ? "https" : "http";
        //var scheme = request.IsHttps ? "https" : "http";
        var location = new Uri($"https://{newHost}");
        return location.AbsoluteUri;
    }
}

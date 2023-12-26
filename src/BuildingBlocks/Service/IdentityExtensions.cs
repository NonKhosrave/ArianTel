using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Service;
public static class IdentityExtensions
{
    public static string GetUserId(this IIdentity identity)
    {
        return identity?.GetUserClaimValue(ClaimTypes.NameIdentifier);
    }

    public static string GetUserClaimValue(this IIdentity identity, string claimType)
    {
        var identity1 = identity as ClaimsIdentity;
        return identity1?.FindFirstValue(claimType);
    }

    public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
    {
        return identity?.FindFirst(claimType)?.Value;
    }

    public static string GetUserName(this IIdentity identity)
    {
        return identity?.GetUserClaimValue(ClaimTypes.Name);
    }

    public static T GetUserId<T>(this IIdentity identity) where T : IConvertible
    {
        var firstValue = identity?.GetUserClaimValue(ClaimTypes.NameIdentifier);
        return firstValue != null
            ? (T)Convert.ChangeType(firstValue, typeof(T), CultureInfo.InvariantCulture)
            : default;
    }

    /// <summary>
    ///     IdentityResult errors list to string
    /// </summary>
    public static string DumpErrors(this IdentityResult result, bool useHtmlNewLine = false)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        var results = new StringBuilder();
        if (!result.Succeeded)
        {
            foreach (var errorDescription in result.Errors.Select(x => x.Description))
            {
                if (string.IsNullOrWhiteSpace(errorDescription))
                {
                    continue;
                }

                if (!useHtmlNewLine)
                {
                    results.AppendLine(errorDescription);
                }
                else
                {
                    results.Append(errorDescription).AppendLine("<br/>");
                }
            }
        }

        return results.ToString();
    }
}

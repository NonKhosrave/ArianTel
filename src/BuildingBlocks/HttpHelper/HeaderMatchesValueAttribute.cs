using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace BuildingBlocks.HttpHelper;
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class HeaderMatchesValueAttribute : Attribute, IActionConstraint
{
    private readonly string[] _mediaTypes;
    private readonly string _requestHeaderToMatch;

#pragma warning disable CA1019
    public HeaderMatchesValueAttribute(string requestHeaderToMatch, string[] mediaTypes)
#pragma warning restore CA1019
    {
        _mediaTypes = mediaTypes;
        _requestHeaderToMatch = requestHeaderToMatch;
    }

    //If we want the route matches this attribute this should return true
    public bool Accept(ActionConstraintContext context)
    {
        var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

        return requestHeaders
                   .ContainsKey(_requestHeaderToMatch)
               && _mediaTypes
                   .Select(mediaType =>
                       string.Equals(context.RouteContext.HttpContext.Request
                           .Headers[_requestHeaderToMatch].ToString(), mediaType, StringComparison.OrdinalIgnoreCase))
                   .Any(mediaTypeMatches => mediaTypeMatches);
    }

    //Decide which stage this constraint is belong for
    public int Order => 0;
}

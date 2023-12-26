using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Model;

public readonly struct CallApiRequestContext
{
    public CallApiRequestContext(HttpMethod httpMethod = null, string serviceUrl = null, HttpContent httpContent = null,
        IDictionary<string, string> header = null)
    {
        MethodType = httpMethod ?? HttpMethod.Get;
        Headers = header;
        RequestContent = httpContent;
        ServiceUrl = serviceUrl;
    }

    [JsonIgnore]
    public HttpContent RequestContent { get; init; }

    public HttpMethod MethodType { get; init; }

    public string ServiceUrl { get; init; }

    [JsonIgnore]
    public IDictionary<string, string> Headers { get; init; }
}

//public readonly record struct CallApiResponseContext<TResponse>(TResponse Response = default,
//    string HttpResponseMessage = null, HttpStatusCode StatusCode = HttpStatusCode.RequestTimeout, bool IsSuccessStatusCode = false,
//    bool IsSuccessDeserializeObject = false, string RequestUri = null, Exception Exception = null, string RequestContent = null,
//    Dictionary<string, string> ResponseHeader = null, List<ValidationFailure> ValidationFailures = null) where TResponse : notnull;

public class CallApiResponseContext
{
    public CallApiResponseContext()
    {
        IsSuccessStatusCode = false;
        IsSuccessDeserializeObject = false;
        StatusCode = HttpStatusCode.RequestTimeout;
        ResponseHeader = new Dictionary<string, string>();
        ValidationFailures = new List<ValidationFailure>();
    }

    public string HttpResponseMessage { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public bool IsSuccessStatusCode { get; set; }

    public bool IsSuccessDeserializeObject { get; set; }

    public List<ValidationFailure> ValidationFailures { get; set; }

    public string RequestUri { get; set; }

    [JsonIgnore]
    public Exception Exception { get; set; }

    public string RequestContent { get; set; }

    public Dictionary<string, string> ResponseHeader { get; set; }
}

public sealed class CallApiResponseContext<TResponse> : CallApiResponseContext where TResponse : notnull
{
    public TResponse Response { get; set; }
}

public readonly struct ValidationFailure
{
    public ValidationFailure(string propertyName, string errorMessage, int errorCode)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public string PropertyName { get; init; }

    public string ErrorMessage { get; init; }

    public int ErrorCode { get; init; }
}

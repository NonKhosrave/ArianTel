using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Model;

namespace BuildingBlocks.Service;

public static class ServiceCallExtension
{
    public static async Task<CallApiResponseContext<TResponseEntity>> SendAsync<TResponseEntity>(
        this HttpClient client, CallApiRequestContext context, CancellationToken cancellationToken = default)
        where TResponseEntity : notnull
    {
        // 1. Create response 
        var response = new CallApiResponseContext<TResponseEntity>();

        // 3.Create httpRequestMessage
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = context.MethodType;
        if (client.BaseAddress != null)
            httpRequestMessage.RequestUri = new Uri(client.BaseAddress, context.ServiceUrl);
        else
            httpRequestMessage.RequestUri = new Uri(context.ServiceUrl);

        if (context.RequestContent != null)
            httpRequestMessage.Content = context.RequestContent;

        if (context.Headers != null)
            foreach (var header in context.Headers)
                httpRequestMessage.Headers.Add(header.Key, header.Value);

        try
        {
            // 4.Send request
            using var httpResponseMessage = await client.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            response.HttpResponseMessage = await httpResponseMessage?.Content?.ReadAsStringAsync(cancellationToken);
            response.ResponseHeader =
                httpResponseMessage?.Headers?.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
            response.StatusCode = httpResponseMessage.StatusCode;
            response.IsSuccessStatusCode = httpResponseMessage?.IsSuccessStatusCode ?? false;
            response.RequestUri = httpRequestMessage.RequestUri?.ToString();

            // ** Get RequestContent
            if (httpRequestMessage.Content != null)
                response.RequestContent = await httpRequestMessage.Content?.ReadAsStringAsync(cancellationToken);

            // 5.DeserializeObject
            if (response.IsSuccessStatusCode)
            {
                response.Response = response.HttpResponseMessage.ToObject<TResponseEntity>();
                response.IsSuccessDeserializeObject = true;
            }
        }
        // UnKnown or undefined exception
        catch (Exception exp)
        {
            var failur = new ValidationFailure("UnKnown",
                $"ServiceCall has Exception . response: {response.ToJson()} exception {exp.Message}.", -1);
            response.ValidationFailures.Add(failur);
            response.Exception = exp;
        }
        finally
        {
            context.RequestContent?.Dispose();
        }

        // 7.Return response
        return response;
    }


    public static async Task<CallApiResponseContext> SendAsync(this HttpClient client,
        CallApiRequestContext context, CancellationToken cancellationToken = default)
    {
        // 1. Create response 
        var response = new CallApiResponseContext();

        // 3.Create httpRequestMessage
        using var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Method = context.MethodType;
        if (client.BaseAddress != null)
            httpRequestMessage.RequestUri = new Uri(client.BaseAddress, context.ServiceUrl);
        else
            httpRequestMessage.RequestUri = new Uri(context.ServiceUrl);

        if (context.RequestContent != null)
            httpRequestMessage.Content = context.RequestContent;

        if (context.Headers != null)
            foreach (var header in context.Headers)
                httpRequestMessage.Headers.Add(header.Key, header.Value);

        try
        {
            // 4.Send request
            using var httpResponseMessage = await client.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            response.HttpResponseMessage = await httpResponseMessage?.Content?.ReadAsStringAsync(cancellationToken);
            response.ResponseHeader =
                httpResponseMessage?.Headers?.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
            response.StatusCode = httpResponseMessage.StatusCode;
            response.IsSuccessStatusCode = httpResponseMessage?.IsSuccessStatusCode ?? false;
            response.RequestUri = httpRequestMessage.RequestUri?.ToString();

            // ** Get RequestContent
            if (httpRequestMessage.Content != null)
                response.RequestContent = await httpRequestMessage.Content?.ReadAsStringAsync(cancellationToken);
        }
        // UnKnown or undefined exception
        catch (Exception exp)
        {
            var failur = new ValidationFailure("UnKnown",
                $"ServiceCall has Exception . response: {response.ToJson()} exception {exp.Message}.", -1);
            response.ValidationFailures.Add(failur);
            response.Exception = exp;
        }
        finally
        {
            context.RequestContent?.Dispose();
        }

        // 7.Return response
        return response;
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Logger;
using BuildingBlocks.Model;
using BuildingBlocks.Service;
using ArianTel.Core.Config;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Services.Ipg;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Service.Commands.Ipg;
using ArianTel.Service.Services.Models.Ipg;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArianTel.Service.Services.Ipg;
public sealed class SamanProviderService : IIpgGatewayService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediator _mediator;
    private readonly AppConfig _config;
    private readonly ILogger<SamanProviderService> _logger;

    public SamanProviderService(IHttpClientFactory httpClientFactory, IOptions<AppConfig> config, ILogger<SamanProviderService> logger, IMediator mediator)
    {
        _httpClientFactory = httpClientFactory;
        _config = config.Value;
        _logger = logger;
        _mediator = mediator;
    }

    public Bank Bank => Bank.Saman;
    public async Task<InitIpgModelResponse> InitIpg(InitIpgModelRequest request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("SamanProvider");
        var requestBody = request.Adapt<InitIpgProviderRequest>();
        requestBody.RedirectUrl = _config.SamanProviderSettings.CallBackUrl;
        var terminalId = _config.SamanProviderSettings.TerminalId;
        requestBody.TerminalId = terminalId;
        var callApiRequestContext = new CallApiRequestContext(HttpMethod.Post, _config.SamanProviderSettings.InitIpgUri, httpContent: requestBody.ToHttpContent());

        var response = await httpClient.SendAsync<InitIpgProviderResponse>(callApiRequestContext, cancellationToken);
        var serviceRes = await InitExceptionHandling<InitIpgProviderResponse, InitIpgModelResponse>(response, callApiRequestContext);

        serviceRes.HttpMethod = HttpMethod.Post;
        serviceRes.Url = _config.SamanProviderSettings.GateWayUrl;
        serviceRes.ExtraData = serviceRes.HasError ? response?.ToJson() : null;
        serviceRes.HtmlString = GenerateHtml(serviceRes);
        serviceRes.TerminalId = terminalId;
        serviceRes.CallBackUrl = requestBody.RedirectUrl;
        return serviceRes;
    }

    public async Task<CallBackModelResponse> CallBack(CallBackModelRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request.Adapt<SamanCallBackCommandRequest>(), cancellationToken);
        return response.Adapt<CallBackModelResponse>();
    }

    public async Task<VerifyIpgModelResponse> VerifyIpg(VerifyIpgModelRequest request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("SamanProvider");
        var requestBody = request.Adapt<VerifyIpgProviderRequest>();
        var callApiRequestContext = new CallApiRequestContext(HttpMethod.Post, _config.SamanProviderSettings.VerifyIpgUri, httpContent: requestBody.ToHttpContent());
        var response = await httpClient.SendAsync<VerifyIpgProviderResponse>(callApiRequestContext, cancellationToken);
        var serviceRes = await ExceptionHandling<VerifyIpgProviderResponse, VerifyIpgModelResponse>(response, callApiRequestContext);

        if (serviceRes.HasError &&
            serviceRes.Error.Code.Equals("TransactionAlreadyVerified", StringComparison.OrdinalIgnoreCase))
            serviceRes.Error = new();

        var transactionStatus =
            GetIpgStatus(response.Response.ResultCode, response.Response.ResultDescription);

        serviceRes.IsSuccessful = transactionStatus.IsSuccessful;
        serviceRes.IpgStatus = transactionStatus.IpgStatus;
        serviceRes.MaskedPan = response.Response?.MaskedPan;
        serviceRes.TerminalId = response.Response?.TerminalNumber.ToString(CultureInfo.InvariantCulture);
        _ = long.TryParse(response.Response?.OriginalAmount.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out var amount);
        serviceRes.Amount = amount;
        serviceRes.ExtraData = serviceRes.HasError ? response.ToJson() : null;
        serviceRes.ProviderMessage = response.Response?.ResultDescription;
        serviceRes.ReferenceNo = response.Response?.RefNum;
        serviceRes.RRN = response.Response?.RRN;
        serviceRes.ReserveNo = request.ReserveNo;
        serviceRes.TraceNo = request.TraceNo;

        return serviceRes;
    }

    public async Task<ReverseIpgModelResponse> ReverseIpg(ReverseIpgModelRequest request, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("SamanProvider");
        var requestBody = request.Adapt<ReverseIpgProviderRequest>();
        var callApiRequestContext = new CallApiRequestContext(HttpMethod.Post, _config.SamanProviderSettings.ReverseIpgUri, httpContent: requestBody.ToHttpContent());
        var response = await httpClient.SendAsync<ReverseIpgProviderResponse>(callApiRequestContext, cancellationToken);
        var serviceRes = await ExceptionHandling<ReverseIpgProviderResponse, ReverseIpgModelResponse>(response, callApiRequestContext);

        var ipgStatus = response.Response.ResultCode == 0 && response.Response.Success
            ? IpgStatus.Succeeded
            : IpgStatus.Failed;
        serviceRes.IsSuccessful = response.Response.ResultCode == 0;
        serviceRes.IpgStatus = ipgStatus;
        serviceRes.ExtraData = serviceRes.HasError ? response.ToJson() : string.Empty;

        return serviceRes;
    }

    private string GenerateHtml(InitIpgModelResponse serviceRes)
    {
        StringBuilder html = new();
        html.Append("<html>");
        html.Append("<head></head>");
        html.Append("<body>");
        html.Append(CultureInfo.InvariantCulture,
            $"<form action=\"{_config.SamanProviderSettings.GateWayUrl}\" name=\"redirect_form\" id=\"redirect_form\" method=\"{HttpMethod.Post}\">");
        html.Append(CultureInfo.InvariantCulture, $"<input type=\"hidden\" name=\"Token\" value=\"{serviceRes.Token}\" />");
        html.Append("<input  type=\"hidden\" name=\"GetMethod\" value=\"false\" />");
        html.Append("</form>");
        html.Append("<script>");
        html.Append("document.getElementById('redirect_form').submit();");
        html.Append("</script>");
        html.Append("</body>");
        html.Append("</html>");
        return html.ToString();
    }

    private async Task<TRes> InitExceptionHandling<TResProvider, TRes>(CallApiResponseContext<TResProvider> context, CallApiRequestContext request)
        where TRes : ServiceResContextBase, new()
        where TResProvider : BaseErrorModel, new()
    {
        try
        {
            if (!context.IsSuccessStatusCode || context.Exception != null || !string.IsNullOrEmpty(context.Response.ErrorCode))
            {
                var error = _config.SamanProviderSettings.ErrorCode;
                if (context.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var errorDetail =
                        error.FirstOrDefault(r => r.Key.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase));

                    return new TRes
                    {
                        Error = new ValidationError("Unauthorized", errorDetail.Value)
                    };
                }

                _logger.CompileLog(context.Exception, LogLevel.Error,
                    $"Saman Ipg service error request: {request.ToJson()}, res: {context.ToJson()}");

                BaseErrorModel errorResponse = null;
                if (!string.IsNullOrEmpty(context.HttpResponseMessage))
                    errorResponse = context.HttpResponseMessage.ToObject<BaseErrorModel>();

                var errorRes =
                    await GetError(errorResponse?.ErrorCode, errorResponse?.ErrorDesc);

                context.Response ??= new TResProvider();

                context.Response.Error = new ValidationError(errorRes.ErrorCode, errorRes.ErrorMessage);
            }

            return context.Response.Adapt<TRes>();
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error,
                $"Saman Ipg service error request: {request.ToJson()}, res: {context.ToJson()}");
            throw;
        }
    }

    private async Task<TRes> ExceptionHandling<TResProvider, TRes>(CallApiResponseContext<TResProvider> context, CallApiRequestContext request)
        where TRes : ServiceResContextBase, new()
        where TResProvider : SamanProviderModelBaseResponse, new()
    {
        try
        {
            if (!context.IsSuccessStatusCode || context.Exception != null || context.Response?.ResultCode != 0 || !context.Response.Success)
            {
                var error = _config.SamanProviderSettings.ErrorCode;
                if (context.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var errorDetail =
                        error.FirstOrDefault(r => r.Key.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase));

                    return new TRes
                    {
                        Error = new ValidationError("Unauthorized", errorDetail.Value)
                    };
                }

                _logger.CompileLog(context.Exception, LogLevel.Error,
                    $"Saman Ipg service error request: {request.ToJson()}, res: {context.ToJson()}");

                SamanProviderModelBaseResponse errorResponse = null;
                if (!string.IsNullOrEmpty(context.HttpResponseMessage))
                    errorResponse = context.HttpResponseMessage.ToObject<SamanProviderModelBaseResponse>();

                var errorRes =
                    await GetError(errorResponse?.ResultCode.ToString(CultureInfo.InvariantCulture)
                        , errorResponse?.ResultDescription);

                context.Response ??= new TResProvider();

                context.Response.Error = new ValidationError(errorRes.ErrorCode, errorRes.ErrorMessage);
            }

            return context.Response.Adapt<TRes>();
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error,
                $"Saman Ipg service error request: {request.ToJson()}, res: {context.ToJson()}");
            throw;
        }
    }

    private async Task<(string ErrorCode, string ErrorMessage)> GetError(string providerErrorCode, string providerErrorMessage)
    {
        var exceptionCommandRs =
            await _mediator.Send(new IpgExceptionsCommandRequest { Bank = Bank.Saman });

        var error = exceptionCommandRs
            ?.Items
            ?.FirstOrDefault(e => e.ProviderErrorCode.Equals(providerErrorCode, StringComparison.OrdinalIgnoreCase)
                                  &&
                                  e.ProviderErrorMessageFa.Equals(providerErrorMessage, StringComparison.OrdinalIgnoreCase));

        return (error?.Code ?? "ProviderError", error?.Message ?? "خطای سرویس دهنده.");
    }

    private static (bool IsSuccessful, IpgStatus IpgStatus) GetIpgStatus(int? providerResultCode, string providerResultMsg)
    {
        return providerResultCode switch
        {
            { } a when a == 0 && (providerResultMsg?.Equals("عملیات با موفقیت انجام شد", StringComparison.OrdinalIgnoreCase) ?? false) => (true, IpgStatus.Succeeded),

            { } b when b == -1 => (false, IpgStatus.Failed),

            { } c when c == -6 => (false, IpgStatus.Failed),

            { } d when d == 2 && (providerResultMsg?.Equals("تراکنش پیش از این تایید شده است.", StringComparison.OrdinalIgnoreCase) ?? false)
                => (true, IpgStatus.Succeeded),

            _ => (false, IpgStatus.Failed)
        };
    }
}

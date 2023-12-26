using System.Net.Http;
using BuildingBlocks.Domain.Services;

namespace ArianTel.Core.Services.Models.Ipg;
public sealed class InitIpgModelRequest
{
    public string Action { get; set; } = "Token";
    public long Amount { get; set; }
    public string TerminalId { get; set; }
    public string RedirectUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string TrackingCode { get; set; }
    public long TerminalInitIpgRequestId { get; set; }
    public string InvoiceId { get; set; }
    public int OrderId { get; set; }
    public string CustomerIp { get; set; }
}

public sealed class InitIpgModelResponse : ServiceResContextBase
{
    public string Url { get; set; }
    public string CallBackUrl { get; set; }
    public string Status { get; set; }
    public string TrackingCode { get; set; }
    public HttpMethod HttpMethod { get; set; }
    public string ExtraData { get; set; }
    public string Token { get; set; }
    public string ProviderMessage { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorDesc { get; set; }
    public string HtmlString { get; set; }
    public string TerminalId { get; set; }
}

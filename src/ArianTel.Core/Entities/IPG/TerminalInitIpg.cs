using System;
using ArianTel.Core.Abstractions;

namespace ArianTel.Core.Entities.Ipg;
public sealed class TerminalInitIpgRequest : DbBaseEntity<long>
{
    public string InvoiceId { get; set; }
    public int BankCode { get; set; }
    public long Amount { get; set; }
    public string PhoneNumber { get; set; }
    public string AdditionalData { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string UserAgent { get; set; }
    public string CustomerIp { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}

public sealed class TerminalInitIpgResponse : IDbBaseEntity
{
    public long TerminalInitIpgRequestId { get; set; }
    public string CallBackUrl { get; set; }
    public string UrlRedirectForm { get; set; }
    public bool IsSuccessful { get; set; }
    public string HttpMethod { get; set; }
    public string Token { get; set; }
    public string TerminalId { get; set; }
    public string InvoiceId { get; set; }
    public long Amount { get; set; }
    public string PhoneNumber { get; set; }
    public string TrackingCode { get; set; }
    public byte BankCode { get; set; }
    public string ExtraData { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string ProviderMessage { get; set; }
    public int OrderId { get; set; }
    public string CustomerIp { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}

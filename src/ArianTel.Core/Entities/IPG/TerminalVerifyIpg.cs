using System;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Entities.Ipg;
public sealed class TerminalVerifyIpgRequest : DbBaseEntity<long>
{
    public long TerminalInitIpgRequestId { get; set; }
    public string InvoiceId { get; set; }
    public string ReferenceNo { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string TrackingCode { get; set; }
    public string Token { get; set; }
    public string RRN { get; set; }
    public string ReserveNo { get; set; }
    public string TraceNo { get; set; }
    public int OrderId { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
    public string UpdateBy { get; set; }
}

public sealed class TerminalVerifyIpgResponse : DbBaseEntity<long>
{
    public long TerminalInitIpgRequestId { get; set; }
    public bool IsSuccessful { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string InvoiceId { get; set; }
    public string TrackingCode { get; set; }
    public string Token { get; set; }
    public string RRN { get; set; }
    public string ReferenceNo { get; set; }
    public string ReserveNo { get; set; }
    public string TraceNo { get; set; }
    public string MaskedPan { get; set; }
    public string ExtraData { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public IpgStatus IpgStatus { get; set; }
    public byte BankCode { get; set; }
    public string ProviderMessage { get; set; }
    public int OrderId { get; set; }
    public string PhoneNumber { get; set; }
    public string CustomerIp { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
    public string UpdateBy { get; set; }
}

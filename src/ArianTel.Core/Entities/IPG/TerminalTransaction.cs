using System;
using ArianTel.Core.Abstractions;

namespace ArianTel.Core.Entities.Ipg;
public sealed class TerminalTransaction : DbBaseEntity<long>
{
    public long TerminalInitIpgRequestId { get; set; }
    public bool HasSucceededVerify { get; set; }
    public bool HasSucceededCallBack { get; set; }
    public string PhoneNumber { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string InvoiceId { get; set; }
    public string TrackingCode { get; set; }
    public string RRN { get; set; }
    public string ReferenceNo { get; set; }
    public string TraceNo { get; set; }
    public string MaskedPan { get; set; }
    public int IpgStatus { get; set; }
    public string ProviderMessage { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public int OrderId { get; set; }
    public string CustomerIp { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
    public string UpdateBy { get; set; }
}

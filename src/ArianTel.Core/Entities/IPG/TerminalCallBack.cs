using System;
using ArianTel.Core.Abstractions;

namespace ArianTel.Core.Entities.Ipg;
public sealed class TerminalCallBack : IDbBaseEntity
{
    public long TerminalInitIpgRequestId { get; set; }
    public bool IsSuccessful { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string RRN { get; set; }
    public string ReferenceNo { get; set; }
    public string ReserveNo { get; set; }
    public string TraceNo { get; set; }
    public string ProviderStatus { get; set; }
    public string TrackingCode { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public int BankCode { get; set; }
    public string Token { get; set; }
    public int OrderId { get; set; }
    public string SecurePan { get; set; }
    public DateTime InsertDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}

using System;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Services.Models.Ipg;
public sealed class CallBackModelRequest
{
    public string MID { get; set; }
    public string State { get; set; }
    public int? Status { get; set; }
    public string RRN { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string TerminalId { get; set; }
    public string TraceNo { get; set; }
    public long Amount { get; set; }
    public long? Wage { get; set; }
    public string SecurePan { get; set; }
    public string HashedCardNumber { get; set; }
    public long TerminalInitIpgRequestId { get; set; }
}

public sealed class CallBackModelResponse : ServiceResContextBase
{
    public bool IsSuccessful { get; set; }
    public string RRN { get; set; }
    public long Amount { get; set; }
    public string TraceNo { get; set; }
    public string SecurePan { get; set; }
    public string ReserveNo { get; set; }
    public string ReferenceNo { get; set; }
    public string HashedCardNo { get; set; }
    public string ProviderStatus { get; set; }
    public IpgStatus IpgStatus { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public string CustomerIp { get; set; }
}

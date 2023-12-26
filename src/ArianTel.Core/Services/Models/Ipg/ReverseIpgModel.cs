using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Services.Models.Ipg;
public sealed class ReverseIpgModelRequest
{
    public string TrackingCode { get; set; }
    public string TerminalId { get; set; }
}

public sealed class ReverseIpgModelResponse : ServiceResContextBase
{
    public TransactionDetail TransactionDetail { get; set; }
    public bool IsSuccessful { get; set; }
    public IpgStatus IpgStatus { get; set; }
    public string ExtraData { get; set; }
}

public sealed class TransactionDetail
{
    public string Rrn { get; set; }
    public string RefNum { get; set; }
    public string MaskedPan { get; set; }
    public string HashedPan { get; set; }
    public string TerminalNumber { get; set; }
    public long OriginalAmount { get; set; }
    public long AffectiveAmount { get; set; }
    public string StraceDate { get; set; }
    public string StraceNo { get; set; }
    public bool Succeed { get; set; }
    public string TraceNo { get; set; }
}

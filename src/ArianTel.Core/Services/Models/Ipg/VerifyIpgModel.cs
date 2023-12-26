using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Services.Models.Ipg;
public sealed class VerifyIpgModelRequest
{
    public string TerminalId { get; set; }
    public string InvoiceId { get; set; }
    public string Token { get; set; }
    public string RRN { get; set; }
    public string TraceNo { get; set; }
    public string ReserveNo { get; set; }
    public string ReferenceNo { get; set; }
}

public sealed class VerifyIpgModelResponse : ServiceResContextBase
{
    public IpgStatus IpgStatus { get; set; }
    public bool IsSuccessful { get; set; }
    public long Amount { get; set; }
    public string MaskedPan { get; set; }
    public string ExtraData { get; set; }
    public string ProviderMessage { get; set; }
    public string TerminalId { get; set; }
    public string HashedCardNo { get; set; }
    public string RRN { get; set; }
    public string ReserveNo { get; set; }
    public string ReferenceNo { get; set; }
    public string TraceNo { get; set; }
}

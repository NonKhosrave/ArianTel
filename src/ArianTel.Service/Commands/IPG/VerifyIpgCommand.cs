using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;
using MediatR;

namespace ArianTel.Service.Commands.Ipg;
public sealed class VerifyIpgCommandRequest : BaseEntity, IRequest<VerifyIpgCommandResponse>
{
    public long TerminalInitIpgRequestId { get; set; }
    public string PhoneNumber { get; set; }
    public long Amount { get; set; }
    public string TrackingCode { get; set; }
    public string TradeCode { get; set; }
    public byte BankCode { get; set; }
    public string MaskedPan { get; set; }
    public string TerminalId { get; set; }
    public string InvoiceId { get; set; }
    public string Token { get; set; }
    public string RRN { get; set; }
    public string TraceNo { get; set; }
    public string ReserveNo { get; set; }
    public string ReferenceNo { get; set; }
    public int OrderId { get; set; }
    public string CustomerIp { get; set; }
}

public sealed class VerifyIpgCommandResponse : ServiceResContextBase
{
    public IpgStatus IpgStatus { get; set; }
    public string TerminalId { get; set; }
    public long Amount { get; set; }
    public string RRN { get; set; }
    public string TraceNo { get; set; }
    public string ReserveNo { get; set; }
    public string ReferenceNo { get; set; }
    public string MaskedPan { get; set; }
}

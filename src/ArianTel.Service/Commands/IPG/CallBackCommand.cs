using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;
using MediatR;

namespace ArianTel.Service.Commands.Ipg;
public sealed class CallBackCommandRequest : BaseEntity, IRequest<CallBackCommandResponse>
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
    public Bank Bank { get; set; }
    public long TerminalInitIpgRequestId { get; set; }
    public string CustomerIp { get; set; }
}

public sealed class CallBackCommandResponse : ServiceResContextBase
{
    public CallBackCommandResponse()
    {
    }
    public CallBackCommandResponse(string redirectUrl)
    {
        RedirectUrl = redirectUrl;
    }
    public string RedirectUrl { get; set; }
}

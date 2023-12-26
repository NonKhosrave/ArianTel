using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace ArianTel.Service.Commands.Ipg;
public sealed class ReverseIpgCommandRequest : BaseEntity, IRequest<ReverseIpgCommandResponse>
{
    public string TrackingCode { get; set; }
}

public sealed class ReverseIpgCommandResponse : ServiceResContextBase
{
    public string RetrievalRefNo { get; set; }
    public long Amount { get; set; }
}

using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace ArianTel.Service.Commands.Ipg;
public sealed class InitIpgCommandRequest : BaseEntity, IRequest<InitIpgCommandResponse>
{
    public int UserId { get; set; }
    public string InvoiceId { get; set; }
    public long Amount { get; set; }
    public string PhoneNumber { get; set; }
    public string AdditionalData { get; set; }
    public int OrderId { get; set; }
    public string CustomerIp { get; set; }
    public string UserAgent { get; set; }
}

public sealed class InitIpgCommandResponse : ServiceResContextBase
{
    public string HtmlString { get; set; }
}

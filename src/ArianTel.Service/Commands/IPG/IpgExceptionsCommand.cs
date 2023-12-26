using System.Collections.Generic;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;
using MediatR;

namespace ArianTel.Service.Commands.Ipg;
public sealed class IpgExceptionsCommandRequest : BaseEntity, IRequest<IpgExceptionsCommandResponse>
{
    public Bank Bank { get; set; }
}

public sealed class IpgExceptionsCommandResponse : ServiceResContextBase
{
    public List<ExceptionInfo> Items { get; set; } = new List<ExceptionInfo>();
}

public sealed class ExceptionInfo
{
    public string Code { get; set; }
    public string Message { get; set; }
    public byte BankCode { get; set; }
    public string ProviderErrorCode { get; set; }
    public string ProviderErrorMessageEn { get; set; }
    public string ProviderErrorMessageFa { get; set; }
}

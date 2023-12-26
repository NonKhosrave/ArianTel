using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace ArianTel.Service.Commands.Wallet;
public sealed class GetBalanceByIdCommandRequest : BaseEntity, IRequest<GetBalanceByIdCommandResponse>
{
    public int UserId { get; set; }
}

public sealed class GetBalanceByIdCommandResponse : ServiceResContextBase
{
    public decimal Balance { get; set; }
}

using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Wallet;
using MediatR;

namespace ArianTel.Service.Commands.Wallet;
public sealed class ChargeByReferenceWalletCommandRequest : BaseEntity, IRequest<ChargeByReferenceWalletCommandResponse>
{
    public decimal Amount { get; set; }
    public int DesUserId { get; set; }
    public string Description { get; set; }
    public BalanceOperation Operation => BalanceOperation.Transfer;
}

public sealed class ChargeByReferenceWalletCommandResponse : ServiceResContextBase
{
}

using System;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Wallet;
using MediatR;

namespace ArianTel.Service.Commands.Wallet.Core;
public sealed class TransferBalanceCommandRequest : BaseEntity, IRequest<TransferBalanceCommandResponse>
{
    public int? SrcUserId { get; set; }
    public decimal Amount { get; set; }
    public int? DesUserId { get; set; }
    public string Description { get; set; }
    public BalanceOperation Operation => BalanceOperation.Transfer;
    public DateTime CreateDateTime => DateTime.Now;
}

public sealed class TransferBalanceCommandResponse : ServiceResContextBase
{
}

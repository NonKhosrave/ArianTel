using System;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Wallet;
using MediatR;

namespace ArianTel.Service.Commands.Wallet.Core;
public sealed class ChargeReferenceWalletCommandRequest : BaseEntity, IRequest<ChargeReferenceWalletCommandResponse>
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public BalanceOperation Operation => BalanceOperation.Charge;
    public DateTime CreateDateTime => DateTime.Now;
    public Guid TrackingNo => Guid.NewGuid();
}

public sealed class ChargeReferenceWalletCommandResponse : ServiceResContextBase
{
}



using System;
using System.Collections.Generic;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Wallet;
using MediatR;

namespace ArianTel.Service.Commands.Wallet;
public sealed class GetTransactionReportCommandRequest : BaseEntity, IRequest<GetTransactionReportCommandResponse>
{
    public BalanceOperation OperationType { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

public sealed class GetTransactionReportCommandResponse : ServiceResContextBase
{
    public List<TransactionItem> Items { get; set; } = new List<TransactionItem>();
}

public sealed class TransactionItem
{
    public int SrcAccountId { get; set; }
    public int DesAccountId { get; set; }
    public decimal? SrcBalance { get; set; }
    public decimal DesBalance { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public BalanceOperation Operation { get; set; }
    public DateTime CreateDateTime { get; set; }
}

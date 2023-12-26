using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.Service.Commands.Wallet;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Wallet;
public sealed class GetTransactionReportCommandHandler : BaseRequestHandler<GetTransactionReportCommandRequest, GetTransactionReportCommandResponse>
{
    private readonly ITransactionRepository _transactions;
    public GetTransactionReportCommandHandler(ITransactionRepository transactions)
    {
        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
    }

    protected override async Task<GetTransactionReportCommandResponse> HandleCore(GetTransactionReportCommandRequest request, CancellationToken cancellationToken)
    {
        var transactions = await _transactions.GetTransactionReport(cancellationToken);

        if (transactions == null)
            return null;

        var items = transactions.Select(t => new TransactionItem()
        {
            SrcAccountId = t.SrcAccountId,
            DesAccountId = t.DesAccountId,
            SrcBalance = t.SrcBalance,
            DesBalance = t.DesBalance,
            Amount = t.Amount,
            CreateDateTime = t.CreateDateTime,
            Description = t.Description,
            Operation = t.Operation,
        });


        items = items.Skip(request.PageIndex * request.PageSize).Take(request.PageSize);
        var result = items.ToList();
        return new GetTransactionReportCommandResponse { Items = result.Adapt<List<TransactionItem>>() };
    }
}

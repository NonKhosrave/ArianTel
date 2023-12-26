using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.Service.Commands.Wallet;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Wallet;
public sealed class GetBalanceByIdCommandHandler : BaseRequestHandler<GetBalanceByIdCommandRequest, GetBalanceByIdCommandResponse>
{
    private readonly IAccountRepository _accounts;
    public GetBalanceByIdCommandHandler(IAccountRepository accounts)
    {
        _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
    }

    protected override async Task<GetBalanceByIdCommandResponse> HandleCore(GetBalanceByIdCommandRequest request, CancellationToken cancellationToken)
    {
        var account = await _accounts.GetAccountById(request.UserId, cancellationToken);

        return account.Adapt<GetBalanceByIdCommandResponse>();
    }
}

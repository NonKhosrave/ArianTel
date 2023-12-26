using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Entities.Wallet;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.Service.Commands.Wallet.Core;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Wallet.Core;
public sealed class ChargeReferenceWalletCommandHandler : BaseRequestHandler<ChargeReferenceWalletCommandRequest, ChargeReferenceWalletCommandResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransferRepository _transferRepository;
    public ChargeReferenceWalletCommandHandler(IAccountRepository accounts, ITransactionRepository transactions, ITransferRepository transfer)
    {
        _accountRepository = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _transactionRepository = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _transferRepository = transfer ?? throw new ArgumentNullException(nameof(transfer));
    }

    protected override async Task<ChargeReferenceWalletCommandResponse> HandleCore(ChargeReferenceWalletCommandRequest request, CancellationToken cancellationToken)
    {
        return await _accountRepository.UnitOfWork.ExecuteTransactionalAsync(async () =>
        {
            var desAccount = await ChargeAsync(null, request.Amount, cancellationToken);

            var transferModel = request.Adapt<Transfer>();
            transferModel.AccountId = desAccount.Id;
            transferModel.Balance = desAccount.Balance;
            await _transferRepository.AddAsync(transferModel, cancellationToken);
            await _transferRepository.UnitOfWork.SaveChangesAsync(cancellationToken);


            var transactionModel = request.Adapt<Transaction>();
            transactionModel.SrcAccountId = desAccount.Id;
            transactionModel.DesAccountId = desAccount.Id;
            transactionModel.DesBalance = desAccount.Balance;
            await _transactionRepository.AddAsync(transactionModel, cancellationToken);
            await _transactionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);


            return new ChargeReferenceWalletCommandResponse();
        });
    }

    private async Task<Account> ChargeAsync(int? userId, decimal amount, CancellationToken cancellationToken)
    {
        var model = await _accountRepository.GetAccountById(userId, cancellationToken);

        if (model != null)
        {
            model.Balance += amount;
            _accountRepository.Update(model);
        }
        else
        {
            model = new Account { UserId = userId };
            model.Balance = amount;
            await _accountRepository.AddAsync(model, cancellationToken);
        }
        await _accountRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return model;
    }
}

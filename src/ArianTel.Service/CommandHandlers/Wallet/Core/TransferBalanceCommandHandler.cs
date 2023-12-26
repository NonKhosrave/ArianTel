using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Entities.Wallet;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.Service.Commands.Wallet.Core;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Wallet.Core;
public sealed class TransferBalanceCommandHandler : BaseRequestHandler<TransferBalanceCommandRequest, TransferBalanceCommandResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransferRepository _transferRepository;
    public TransferBalanceCommandHandler(IAccountRepository accounts, ITransactionRepository transactions, ITransferRepository transfer)
    {
        _accountRepository = accounts ?? throw new ArgumentNullException(nameof(accounts));
        _transactionRepository = transactions ?? throw new ArgumentNullException(nameof(transactions));
        _transferRepository = transfer ?? throw new ArgumentNullException(nameof(transfer));
    }
    protected override async Task<TransferBalanceCommandResponse> HandleCore(TransferBalanceCommandRequest request, CancellationToken cancellationToken)
    {
        return await _accountRepository.UnitOfWork.ExecuteTransactionalAsync(async () =>
        {
            var account = await _accountRepository.GetAccountById(request.SrcUserId, cancellationToken);
            if (account == null || account.Balance <= request.Amount)
            {
                return new TransferBalanceCommandResponse() { Error = new ValidationError("NotEnoughBalance", "به علت عدم موجودي امكان ادامه عمليات وجود ندارد.") };
            }

            var srcAccount = await ChargeAsync(request.SrcUserId, decimal.Negate(request.Amount), cancellationToken);
            var desAccount = await ChargeAsync(request.DesUserId, request.Amount, cancellationToken);

            var trackinNo = Guid.NewGuid();
            await _transferRepository.AddAsync(TransferModelCreation(trackinNo, srcAccount.Id, srcAccount.Balance, request.CreateDateTime, decimal.Negate(request.Amount)), cancellationToken);
            await _transferRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await _transferRepository.AddAsync(TransferModelCreation(trackinNo, desAccount.Id, desAccount.Balance, request.CreateDateTime, request.Amount), cancellationToken);
            await _transferRepository.UnitOfWork.SaveChangesAsync(cancellationToken);


            var transactionModel = request.Adapt<Transaction>();
            transactionModel.SrcAccountId = srcAccount.Id;
            transactionModel.DesAccountId = desAccount.Id;
            transactionModel.SrcBalance = srcAccount.Balance;
            transactionModel.DesBalance = desAccount.Balance;
            await _transactionRepository.AddAsync(transactionModel, cancellationToken);
            await _transactionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new TransferBalanceCommandResponse();
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

    private static Transfer TransferModelCreation(Guid trackinNo, int accountId, decimal balance, DateTime createDateTime, decimal amount)
    {

        return new Transfer
        {
            AccountId = accountId,
            Balance = balance,
            TrackingNo = trackinNo,
            Amount = amount,
            CreateDateTime = createDateTime,
        };
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Core.Services.Ipg;
using ArianTel.Service.Commands.Ipg;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class VerifyIpgCommandHandler : BaseRequestHandler<VerifyIpgCommandRequest, VerifyIpgCommandResponse>
{
    private readonly IIpgFactoryService _factoryService;
    private readonly ITerminalVerifyIpgRequestRepository _terminalVerifyIpgRequestRepository;
    private readonly ITerminalVerifyIpgResponseRepository _terminalVerifyIpgResponseRepository;
    private readonly ITerminalTransactionRepository _terminalTransactionRepository;
    //private readonly IOrderRepository _orderRepository;

    public VerifyIpgCommandHandler(
        IIpgFactoryService factoryService,
        ITerminalVerifyIpgRequestRepository terminalVerifyIpgRequestRepository,
        ITerminalVerifyIpgResponseRepository terminalVerifyIpgResponseRepository,
        ITerminalTransactionRepository terminalTransactionRepository)
    {
        _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        _terminalVerifyIpgRequestRepository = terminalVerifyIpgRequestRepository ?? throw new ArgumentNullException(nameof(terminalVerifyIpgRequestRepository));
        _terminalVerifyIpgResponseRepository = terminalVerifyIpgResponseRepository ?? throw new ArgumentNullException(nameof(terminalVerifyIpgResponseRepository));
        _terminalTransactionRepository = terminalTransactionRepository ?? throw new ArgumentNullException(nameof(terminalTransactionRepository));
        //_orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    protected override async Task<VerifyIpgCommandResponse> HandleCore(VerifyIpgCommandRequest request, CancellationToken cancellationToken)
    {
        var service = _factoryService.GetInstance((Bank)request.BankCode);
        if (service is null)
            return Failure("InvalidRequest", "Invalid BankCode!");

        if (string.IsNullOrEmpty(request.ReferenceNo) || string.IsNullOrEmpty(request.TerminalId))
            return Failure("InvalidRequest", "Saman ReferenceNo or TerminalId not valid.");

        await InsertTerminalVerifyIpgRequestAsync(request, cancellationToken);

        var verifyIpgRqModel = request.Adapt<VerifyIpgModelRequest>();
        var serviceRs = await service.VerifyIpg(verifyIpgRqModel, cancellationToken);

        await ExecuteTransactional(serviceRs, request, cancellationToken);

        return serviceRs.HasError
            ? Failure(serviceRs.Error)
            : serviceRs.Adapt<VerifyIpgCommandResponse>();
    }

    private async Task InsertTerminalVerifyIpgRequestAsync(VerifyIpgCommandRequest model, CancellationToken cancellationToken)
    {
        var dataModel = model.Adapt<TerminalVerifyIpgRequest>();
        await _terminalVerifyIpgRequestRepository.AddAsync(dataModel, cancellationToken);
        await _terminalVerifyIpgRequestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ExecuteTransactional(VerifyIpgModelResponse verifyIpgRs, VerifyIpgCommandRequest initIpgRes, CancellationToken cancellationToken)
    {
        await _terminalVerifyIpgResponseRepository.UnitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await InsertTerminalVerifyIpgResponseAsync(verifyIpgRs, initIpgRes, cancellationToken);
            await UpdateTerminalTransaction(verifyIpgRs, initIpgRes, cancellationToken);
            // await UpdateOrder(initIpgRes.OrderId, verifyIpgRs, cancellationToken);
            return new VerifyIpgModelResponse();
        });
    }
    private async Task InsertTerminalVerifyIpgResponseAsync(VerifyIpgModelResponse verifyIpgRs, VerifyIpgCommandRequest initIpgRes, CancellationToken cancellationToken)
    {
        var dataModel = initIpgRes.Adapt<TerminalVerifyIpgResponse>();
        dataModel.Amount = initIpgRes.Amount;
        dataModel.TerminalId = initIpgRes.TerminalId;
        dataModel.RRN = verifyIpgRs?.RRN;
        dataModel.TraceNo = verifyIpgRs?.TraceNo;
        dataModel.ReserveNo = verifyIpgRs?.ReserveNo;
        dataModel.MaskedPan = verifyIpgRs?.MaskedPan;
        dataModel.ReferenceNo = verifyIpgRs?.ReferenceNo;
        dataModel.ExtraData = verifyIpgRs?.ExtraData;
        dataModel.ErrorCode = verifyIpgRs?.Error.Code;
        dataModel.ErrorMessage = verifyIpgRs?.Error.Message;
        dataModel.ProviderMessage = verifyIpgRs?.ProviderMessage;
        dataModel.IsSuccessful = verifyIpgRs?.IsSuccessful ?? false;
        dataModel.IpgStatus = verifyIpgRs?.IpgStatus ?? IpgStatus.None;
        dataModel.CustomerIp = initIpgRes.CustomerIp;
        await _terminalVerifyIpgResponseRepository.AddAsync(dataModel, cancellationToken);
        await _terminalVerifyIpgResponseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateTerminalTransaction(VerifyIpgModelResponse verifyIpgRs, VerifyIpgCommandRequest initIpgRes, CancellationToken cancellationToken)
    {
        var terminalTransaction = await _terminalTransactionRepository.FirstOrDefaultAsync(predicate: r => r.TerminalInitIpgRequestId == initIpgRes.TerminalInitIpgRequestId && !r.HasSucceededVerify, cancellationToken: cancellationToken);
        if (terminalTransaction != null)
        {
            terminalTransaction.HasSucceededVerify = verifyIpgRs.IsSuccessful;
            terminalTransaction.IpgStatus = (int)verifyIpgRs.IpgStatus;
            terminalTransaction.ProviderMessage = verifyIpgRs.ProviderMessage;
            terminalTransaction.ErrorCode = verifyIpgRs.Error.Code;
            terminalTransaction.ErrorMessage = verifyIpgRs.Error.Message;
            terminalTransaction.MaskedPan = verifyIpgRs.MaskedPan;
            terminalTransaction.UpdateDateTime = DateTime.Now;
            _terminalTransactionRepository.Update(terminalTransaction);
            await _terminalTransactionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    //private async Task UpdateOrder(int orderId, VerifyIpgModelResponse verifyIpg, CancellationToken cancellationToken)
    //{
    //    var order = await _orderRepository.FirstOrDefaultAsync(predicate: r => r.Id == orderId, cancellationToken: cancellationToken);
    //    if (order != null)
    //    {
    //        order.IpgType = !verifyIpg.HasError && verifyIpg.IsSuccessful && verifyIpg.IpgStatus == IpgStatus.Succeeded
    //            ? IpgType.Approved
    //            : IpgType.NotApproved;
    //        _orderRepository.Update(order);
    //        await _orderRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    //    }
    //}
}

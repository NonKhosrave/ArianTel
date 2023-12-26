using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Config;
using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Core.Services.Ipg;
using ArianTel.Service.Commands.Ipg;
using Mapster;
using MediatR;
using Microsoft.Extensions.Options;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class CallBackCommandHandler : BaseRequestHandler<CallBackCommandRequest, CallBackCommandResponse>
{
    private readonly IIpgFactoryService _factoryService;
    private readonly ITerminalInitIpgResponseRepository _terminalInitIpgResponseRepository;
    private readonly IMediator _mediator;
    private readonly ITerminalCallBackRepository _terminalCallBackRepository;
    private readonly ITerminalTransactionRepository _terminalTransactionRepository;
    private readonly AppConfig _appConfig;

    public CallBackCommandHandler(
        IIpgFactoryService factoryService,
        IMediator mediator,
        ITerminalInitIpgResponseRepository terminalInitIpgResponseRepository,
        ITerminalCallBackRepository terminalCallBackRepository,
        ITerminalTransactionRepository terminalTransactionRepository,
        IOptions<AppConfig> options)
    {
        _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _terminalInitIpgResponseRepository = terminalInitIpgResponseRepository ?? throw new ArgumentNullException(nameof(terminalInitIpgResponseRepository));
        _terminalCallBackRepository = terminalCallBackRepository ?? throw new ArgumentNullException(nameof(terminalCallBackRepository));
        _terminalTransactionRepository = terminalTransactionRepository ?? throw new ArgumentNullException(nameof(terminalTransactionRepository));
        _appConfig = options.Value;
    }

    protected override async Task<CallBackCommandResponse> HandleCore(CallBackCommandRequest request, CancellationToken cancellationToken)
    {
        var callBackUrl = _appConfig.GatewaySettings.BankGatewayRequestDeeplink;
        var initIpgRequestInfo =
            await _terminalInitIpgResponseRepository.FindByIdAsync(cancellationToken, request.TerminalInitIpgRequestId);

        if (initIpgRequestInfo is null)
            return Failure("InvalidRequest", "Invalid ResNum.");

        if (!initIpgRequestInfo.IsSuccessful)
            return new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, false, DateTime.Now));

        var service = _factoryService.GetInstance(request.Bank);
        if (service is null)
            return new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, false, DateTime.Now));

        var callBackServiceRes = await service.CallBack(request.Adapt<CallBackModelRequest>(), cancellationToken);

        if (callBackServiceRes.HasError && callBackServiceRes.Error.Code.Equals("DuplicateRequest", StringComparison.OrdinalIgnoreCase))
            return new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, false, callBackServiceRes.TransactionDate));
        callBackServiceRes.CustomerIp = request.CustomerIp;
        await ExecuteTransactional(callBackServiceRes, initIpgRequestInfo, cancellationToken);

        if (callBackServiceRes.IpgStatus != IpgStatus.ComeBackFromCallBack)
            return new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, false, callBackServiceRes.TransactionDate));

        var verifyIpgCommandRq = initIpgRequestInfo.Adapt<VerifyIpgCommandRequest>();
        verifyIpgCommandRq.RRN = callBackServiceRes.RRN;
        verifyIpgCommandRq.TraceNo = callBackServiceRes.TraceNo;
        verifyIpgCommandRq.ReserveNo = callBackServiceRes.ReserveNo;
        verifyIpgCommandRq.MaskedPan = callBackServiceRes.SecurePan;
        verifyIpgCommandRq.ReferenceNo = callBackServiceRes.ReferenceNo;
        verifyIpgCommandRq.CustomerIp = request.CustomerIp;
        var verifyIpgCommandRs = await _mediator.Send(verifyIpgCommandRq, cancellationToken);

        return verifyIpgCommandRs.HasError || verifyIpgCommandRs.IpgStatus != IpgStatus.Succeeded
            ? new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, false, callBackServiceRes.TransactionDate))
            : new CallBackCommandResponse(GenerateCallBackUri(callBackUrl, true, callBackServiceRes.TransactionDate, callBackServiceRes.RRN));
    }

    private async Task ExecuteTransactional(CallBackModelResponse callBackResult, TerminalInitIpgResponse initIpgRes, CancellationToken cancellationToken)
    {
        await _terminalCallBackRepository.UnitOfWork.ExecuteTransactionalAsync(async () =>
        {
            await InsertTerminalCallBackResult(callBackResult, initIpgRes, cancellationToken);
            await InsertTerminalTransaction(callBackResult, initIpgRes, cancellationToken);
            return new CallBackCommandResponse();
        });
    }

    private async Task InsertTerminalCallBackResult(CallBackModelResponse callBackResult, TerminalInitIpgResponse initIpgRes, CancellationToken cancellationToken)
    {
        var dataModel = initIpgRes.Adapt<TerminalCallBack>();
        dataModel.Amount = initIpgRes.Amount;
        dataModel.RRN = callBackResult?.RRN;
        dataModel.TraceNo = callBackResult?.TraceNo;
        dataModel.ReserveNo = callBackResult?.ReserveNo;
        dataModel.ReferenceNo = callBackResult?.ReferenceNo;
        dataModel.ProviderStatus = callBackResult?.ProviderStatus;
        dataModel.IsSuccessful = callBackResult?.IsSuccessful ?? false;
        dataModel.ErrorCode = callBackResult?.Error.Code;
        dataModel.ErrorMessage = callBackResult?.Error.Message;
        dataModel.SecurePan = callBackResult?.SecurePan;

        await _terminalCallBackRepository.AddAsync(dataModel, cancellationToken);
        await _terminalCallBackRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertTerminalTransaction(CallBackModelResponse callBackResult, TerminalInitIpgResponse initIpgRes, CancellationToken cancellationToken)
    {
        var dataModel = initIpgRes.Adapt<TerminalTransaction>();
        dataModel.HasSucceededCallBack = callBackResult.IsSuccessful;
        dataModel.RRN = callBackResult.RRN;
        dataModel.ReferenceNo = callBackResult.ReferenceNo;
        dataModel.TraceNo = callBackResult.TraceNo;
        dataModel.IpgStatus = (int)callBackResult.IpgStatus;
        dataModel.MaskedPan = callBackResult.SecurePan;
        dataModel.CustomerIp = callBackResult.CustomerIp;
        await _terminalTransactionRepository.AddAsync(dataModel, cancellationToken);
        await _terminalTransactionRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _terminalTransactionRepository.Detach(dataModel);
    }

    private static string GenerateCallBackUri(string callBackUrl, bool isSucceeded, DateTime transactionDate, string rrn = null)
        => !string.IsNullOrEmpty(rrn)
            ? $"{callBackUrl}?isSucceeded={isSucceeded}&rrn={rrn}&transactionDate={transactionDate}"
            : $"{callBackUrl}?isSucceeded={isSucceeded}&transactionDate={transactionDate}";
}

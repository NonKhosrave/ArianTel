using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Abstractions.AuditableEntity;
using ArianTel.Core.Config;
using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Core.Services.Ipg;
using ArianTel.Service.Commands.Ipg;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class ReverseIpgCommandHandler : BaseRequestHandler<ReverseIpgCommandRequest, ReverseIpgCommandResponse>
{
    private readonly IIpgFactoryService _factoryService;
    private readonly ITerminalVerifyIpgResponseRepository _terminalVerifyIpgResponseRepository;
    private readonly ITerminalReverseIpgRequestRepository _terminalReverseIpgRequestRepository;
    private readonly ITerminalReverseIpgResponseRepository _terminalReverseIpgResponseRepository;
    private readonly AppConfig _appConfig;

    public ReverseIpgCommandHandler(IIpgFactoryService factoryService, IOptions<AppConfig> appConfig, ITerminalVerifyIpgResponseRepository terminalVerifyIpgResponseRepository, ITerminalReverseIpgRequestRepository terminalReverseIpgRequestRepository, ITerminalReverseIpgResponseRepository terminalReverseIpgResponseRepository)
    {
        _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        _terminalVerifyIpgResponseRepository = terminalVerifyIpgResponseRepository ?? throw new ArgumentNullException(nameof(terminalVerifyIpgResponseRepository));
        _terminalReverseIpgRequestRepository = terminalReverseIpgRequestRepository ?? throw new ArgumentNullException(nameof(terminalReverseIpgRequestRepository));
        _terminalReverseIpgResponseRepository = terminalReverseIpgResponseRepository ?? throw new ArgumentNullException(nameof(terminalReverseIpgResponseRepository));
        _appConfig = appConfig.Value;
    }

    protected override async Task<ReverseIpgCommandResponse> HandleCore(ReverseIpgCommandRequest request, CancellationToken cancellationToken)
    {
        var requestRepo = request.Adapt<ReverseIpgCommandRequest>();

        var responseRepo = await _terminalVerifyIpgResponseRepository.FirstOrDefaultAsync(
            predicate: r => r.TrackingCode == requestRepo.TrackingCode, cancellationToken: cancellationToken);
        if (responseRepo is null)
            return Failure("InvalidRequest", "Invalid TrackingCode!");
        if (!responseRepo.IsSuccessful)
            return Failure("InvalidRequest", "Doesn't Exist TrackingCode!");
        if (responseRepo.IpgStatus != IpgStatus.Succeeded)
            return Failure("InvalidRequest", "Doesn't Exist TrackingCode!");
        if (EF.Property<DateTime>(responseRepo, AuditableShadowProperties.CreatedDateTime) < DateTime.Now.AddMinutes(-_appConfig.SamanProviderSettings.ReverseTimeRange))
            return Failure("InvalidRequest", "Expire TrackingCode!");
        var service = _factoryService.GetInstance((Bank)responseRepo.BankCode);
        if (service is null)
            return Failure("InvalidRequest", "Invalid BankCode!");

        var reqModel = responseRepo.Adapt<TerminalReverseIpgRequest>();
        reqModel.TrackingCode = request.TrackingCode;

        await InsertReverseIpgRequestAsync(reqModel, cancellationToken);

        var model = new ReverseIpgModelRequest()
        {
            TerminalId = responseRepo.TerminalId,
            TrackingCode = responseRepo.ReferenceNo
        };

        var response = await service.ReverseIpg(model, cancellationToken);
        await InsertReverseIpgResponseAsync(response, reqModel, cancellationToken);

        return response.Adapt<ReverseIpgCommandResponse>();
    }

    private async Task InsertReverseIpgRequestAsync(TerminalReverseIpgRequest request, CancellationToken cancellationToken)
    {
        await _terminalReverseIpgRequestRepository.AddAsync(request, cancellationToken);
        await _terminalReverseIpgRequestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task InsertReverseIpgResponseAsync(ReverseIpgModelResponse response, TerminalReverseIpgRequest request, CancellationToken cancellationToken)
    {
        var dataModel = request.Adapt<TerminalReverseIpgResponse>();
        dataModel.Succeed = response.TransactionDetail.Succeed;
        dataModel.ReferenceNo = response.TransactionDetail.Rrn;
        dataModel.MaskedPan = response.TransactionDetail.MaskedPan;
        dataModel.Amount = response.TransactionDetail.OriginalAmount;
        dataModel.TraceNo = response.TransactionDetail.TraceNo;
        dataModel.ErrorCode = response.Error.Code;
        dataModel.ErrorMessage = response.Error.Message;
        dataModel.ExtraData = string.IsNullOrEmpty(response.ExtraData) ? "" : response.ExtraData;

        await _terminalReverseIpgResponseRepository.AddAsync(dataModel, cancellationToken);
        await _terminalReverseIpgResponseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}

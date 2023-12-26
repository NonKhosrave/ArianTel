using System;
using System.Threading;
using System.Threading.Tasks;
using ArianTel.Core.Config;
using ArianTel.Core.Entities.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Core.Services.Ipg;
using ArianTel.Core.Services.Models.Ipg;
using ArianTel.Service.Commands.Ipg;
using BuildingBlocks.Common.Model;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class InitIpgCommandHandler : BaseRequestHandler<InitIpgCommandRequest, InitIpgCommandResponse>
{
    private readonly IIpgFactoryService _factoryService;
    private readonly ITerminalInitIpgRepository _terminalInitIpgRequestRepository;
    private readonly ITerminalInitIpgResponseRepository _terminalInitIpgResponseRepository;
    private readonly AppConfig _appConfig;

    public InitIpgCommandHandler(IIpgFactoryService factoryService,
        ITerminalInitIpgRepository terminalInitIpgRequestRepository,
        ITerminalInitIpgResponseRepository terminalInitIpgResponseRepository,
        IOptions<AppConfig> options)
    {
        _factoryService = factoryService ?? throw new ArgumentNullException(nameof(factoryService));
        _terminalInitIpgRequestRepository = terminalInitIpgRequestRepository ?? throw new ArgumentNullException(nameof(terminalInitIpgRequestRepository));
        _terminalInitIpgResponseRepository = terminalInitIpgResponseRepository ?? throw new ArgumentNullException(nameof(terminalInitIpgResponseRepository));
        _appConfig = options.Value;
    }

    protected override async Task<InitIpgCommandResponse> HandleCore(InitIpgCommandRequest request, CancellationToken cancellationToken)
    {
        request.InvoiceId = Guid.NewGuid().ToString("N");
        var service = _factoryService.GetInstance(_appConfig.GatewaySettings.ActiveGateway);
        if (service is null)
            return Failure("InvalidRequest", "Invalid BankCode!");
        var model = request.Adapt<InitIpgModelRequest>();
        try
        {
            var requestLog = request.Adapt<TerminalInitIpgRequest>();
            requestLog.BankCode = (int)_appConfig.GatewaySettings.ActiveGateway;
            await _terminalInitIpgRequestRepository.AddAsync(requestLog, cancellationToken);
            await _terminalInitIpgRequestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            model.TerminalInitIpgRequestId = requestLog.Id;
        }
        catch (SqlException ex) when (ex.Message.Contains("IX_TerminalInitIpgRequest_InvoiceId_Unique", StringComparison.OrdinalIgnoreCase))
        {
            return Failure("InvalidRequest", "Duplicated InvoiceId.");
        }

        var response = await service.InitIpg(model, cancellationToken);
        response.TrackingCode = model.TrackingCode;

        await InsertInitIpgResponseAsync(response, model, cancellationToken);
        return response.HasError ? Failure(response.Error) : new InitIpgCommandResponse { HtmlString = response.HtmlString };
    }

    private async Task InsertInitIpgResponseAsync(InitIpgModelResponse response, InitIpgModelRequest request, CancellationToken cancellationToken)
    {
        var dataModel = request.Adapt<TerminalInitIpgResponse>();
        dataModel.ErrorCode = response.Error.Code;
        dataModel.IsSuccessful = !response.HasError;
        dataModel.TrackingCode = response.TrackingCode;
        dataModel.ErrorMessage = response.Error.Message;
        dataModel.HttpMethod = response.HttpMethod?.ToString();
        dataModel.BankCode = (byte)_appConfig.GatewaySettings.ActiveGateway;
        dataModel.Token = response.Token;
        dataModel.ProviderMessage = response.ProviderMessage;
        dataModel.ExtraData = response.ExtraData;
        dataModel.TerminalId = response.TerminalId;
        dataModel.UrlRedirectForm = response.Url;
        dataModel.CallBackUrl = response.CallBackUrl;

        await _terminalInitIpgResponseRepository.AddAsync(dataModel, cancellationToken);
        await _terminalInitIpgResponseRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}

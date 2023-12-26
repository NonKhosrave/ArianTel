using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Service.Commands.Ipg;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class SamanCallBackCommandHandler : BaseRequestHandler<SamanCallBackCommandRequest, SamanCallBackCommandResponse>
{
    private readonly ITerminalCallBackRepository _terminalCallBackRepository;

    public SamanCallBackCommandHandler(ITerminalCallBackRepository terminalCallBackRepository)
    {
        _terminalCallBackRepository = terminalCallBackRepository ?? throw new ArgumentNullException(nameof(terminalCallBackRepository));
    }

    protected override async Task<SamanCallBackCommandResponse> HandleCore(SamanCallBackCommandRequest request, CancellationToken cancellationToken)
    {
        var terminalCallBack = await _terminalCallBackRepository.FirstOrDefaultAsync(
            predicate: r => r.TerminalInitIpgRequestId == request.TerminalInitIpgRequestId,
            cancellationToken: cancellationToken);
        if (terminalCallBack is not null)
            return Failure("DuplicateRequest", "The Request Is Duplicate.");

        var ipgStatus = GetIpgStatus(request.State);

        return new SamanCallBackCommandResponse
        {
            IsSuccessful = ipgStatus.IsSuccessful,
            IpgStatus = ipgStatus.Status,
            RRN = request.RRN,
            Amount = request.Amount,
            HashedCardNo = request.HashedCardNumber,
            ProviderStatus = request.State,
            ReferenceNo = request.RefNum,
            ReserveNo = request.ResNum,
            SecurePan = request.SecurePan,
            TraceNo = request.TraceNo,
            Error = ipgStatus.Error ?? new ValidationError()
        };
    }

    private static (bool IsSuccessful, IpgStatus Status, ValidationError? Error) GetIpgStatus(string providerStatus)
    {
        return providerStatus switch
        {
            not null when providerStatus.Equals("OK", StringComparison.OrdinalIgnoreCase)
                => (true, IpgStatus.ComeBackFromCallBack, null),

            not null when providerStatus.Equals("CanceledByUser", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("IpgCancelled", "لغو درخواست توسط کاربر")),

            not null when providerStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, null),

            not null when providerStatus.Equals("SessionIsNull", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("SessionExpired", "کاربر در بازه زمانی تعیین شده، پاسخی ارسال نکرده است.")),

            not null when providerStatus.Equals("TokenNotFound", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("TokenNotFound", "اطلاعات توکن یافت نشد.")),

            not null when providerStatus.Equals("TokenRequired", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("TokenRequired", "برای این شناسه ترمینال، تنها تراکنش های توکنی قابل انجام می باشند.")),

            not null when providerStatus.Equals("InvalidParameters", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("ValidationError", "پارامتر نامعتبر")),

            not null when providerStatus.Equals("TerminalNotFound", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("TerminalNotFound", "شماره ترمینال یافت نشد.")),

            not null when providerStatus.Equals("MerchantIpAddressIsInvalid", StringComparison.OrdinalIgnoreCase)
                => (false, IpgStatus.Failed, new ValidationError("MerchantIpInvalid", "آدرس سرور پذیرنده نامعتبر است.")),

            _ => (false, IpgStatus.Failed, new ValidationError("ProviderError", "خطای سرویس دهنده"))
        };
    }
}

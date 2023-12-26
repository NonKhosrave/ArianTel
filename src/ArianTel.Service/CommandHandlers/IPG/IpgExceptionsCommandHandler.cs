using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Core.Repositories.Ipg;
using ArianTel.Service.Commands.Ipg;
using Mapster;

namespace ArianTel.Service.CommandHandlers.Ipg;
public sealed class IpgExceptionsCommandHandler : BaseRequestHandler<IpgExceptionsCommandRequest, IpgExceptionsCommandResponse>
{
    private readonly IExceptionsRepository _exceptionsRepository;

    public IpgExceptionsCommandHandler(IExceptionsRepository exceptionsRepository)
    {
        _exceptionsRepository = exceptionsRepository;
    }

    protected override async Task<IpgExceptionsCommandResponse> HandleCore(IpgExceptionsCommandRequest request, CancellationToken cancellationToken)
    {
        var exceptions = await _exceptionsRepository.FindAsync(predicate: r => r.BankCode == (byte)request.Bank, cancellationToken: cancellationToken);
        return new IpgExceptionsCommandResponse
        {
            Items = exceptions?.Adapt<List<ExceptionInfo>>()
        };
    }
}

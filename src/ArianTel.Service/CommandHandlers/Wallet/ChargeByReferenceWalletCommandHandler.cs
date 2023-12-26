using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using ArianTel.Service.Commands.Wallet;
using ArianTel.Service.Commands.Wallet.Core;
using Mapster;
using MediatR;

namespace ArianTel.Service.CommandHandlers.Wallet;
public sealed class ChargeByReferenceWalletCommandHandler : BaseRequestHandler<ChargeByReferenceWalletCommandRequest, ChargeByReferenceWalletCommandResponse>
{
    private readonly IMediator _mediator;

    public ChargeByReferenceWalletCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    protected override async Task<ChargeByReferenceWalletCommandResponse> HandleCore(ChargeByReferenceWalletCommandRequest request, CancellationToken cancellationToken)
    {
        var commandRequest = request.Adapt<TransferBalanceCommandRequest>();
        var createResponse = await _mediator.Send(commandRequest, cancellationToken);
        return createResponse.HasError ? Failure(createResponse.Error) : new ChargeByReferenceWalletCommandResponse();
    }
}

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Services.Models.Ipg;

namespace ArianTel.Core.Services.Ipg;
public interface IIpgGatewayService : IScopeLifetime
{
    Bank Bank { get; }
    Task<InitIpgModelResponse> InitIpg(InitIpgModelRequest request, CancellationToken cancellationToken);
    Task<CallBackModelResponse> CallBack(CallBackModelRequest request, CancellationToken cancellationToken);

    Task<VerifyIpgModelResponse> VerifyIpg(VerifyIpgModelRequest request,
        CancellationToken cancellationToken);

    Task<ReverseIpgModelResponse> ReverseIpg(ReverseIpgModelRequest request,
        CancellationToken cancellationToken);
}

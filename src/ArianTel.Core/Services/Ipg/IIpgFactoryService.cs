using BuildingBlocks.DependencyInjection;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Services.Ipg;
public interface IIpgFactoryService : IScopeLifetime
{
    IIpgGatewayService GetInstance(Bank bank);
}

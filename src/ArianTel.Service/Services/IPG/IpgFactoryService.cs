using System.Collections.Generic;
using System.Linq;
using ArianTel.Core.Enums.Ipg;
using ArianTel.Core.Services.Ipg;

namespace ArianTel.Service.Services.Ipg;
public sealed class IpgFactoryService : IIpgFactoryService
{
    private readonly IEnumerable<IIpgGatewayService> _ipgServices;

    public IpgFactoryService(IEnumerable<IIpgGatewayService> ipgServices)
    {
        _ipgServices = ipgServices;
    }

    public IIpgGatewayService GetInstance(Bank bank)
        => _ipgServices?.FirstOrDefault(s => s.Bank == bank);
}

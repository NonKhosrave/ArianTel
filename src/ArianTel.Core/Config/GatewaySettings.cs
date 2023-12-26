using ArianTel.Core.Enums.Ipg;

namespace ArianTel.Core.Config;
public sealed class GatewaySettings
{
    public Bank ActiveGateway { get; set; }
    public string InitIpgUrl { get; set; }
    public string BankGatewayRequestDeeplink { get; set; }
}

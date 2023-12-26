using System.Collections.Generic;

namespace ArianTel.Core.Config.Ipg;
public sealed class SamanProviderSettings
{
    public string BaseUrl { get; set; }
    public string InitIpgUri { get; set; }
    public string VerifyIpgUri { get; set; }
    public string ReverseIpgUri { get; set; }
    public string GateWayUrl { get; set; }
    public string CallBackUrl { get; set; }
    public int TimeOut { get; set; }
    public int ThirdPartyCode { get; set; }
    public Dictionary<string, string> ErrorCode { get; set; }
    public int ReverseTimeRange { get; set; }
    public int VerifyTimeRangeAsMinutes { get; set; }
    public string TerminalId { get; set; }
}

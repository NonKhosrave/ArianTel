using System.Text.Json.Serialization;
using BuildingBlocks.Domain.Services;

namespace ArianTel.Service.Services.Models.Ipg;
public class BaseErrorModel : ServiceResContextBase
{
    public int Status { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorDesc { get; set; }
}

public sealed class InitIpgProviderRequest
{
    public string Action { get; set; }
    public long Amount { get; set; }
    public string TerminalId { get; set; }
    public string ResNum { get; set; }
    public string RedirectUrl { get; set; }
    public string CellNumber { get; set; }
}

public sealed class InitIpgProviderResponse : BaseErrorModel
{
    public string Token { get; set; }
}

public sealed class VerifyIpgProviderRequest
{
    [JsonPropertyName("RefNum")]
    public string ReferenceNo { get; set; }

    [JsonPropertyName("TerminalNumber")]
    public string TerminalId { get; set; }
}

public class SamanProviderModelBaseResponse : ServiceResContextBase
{
    public int ResultCode { get; set; }
    public string ResultDescription { get; set; }
    public bool Success { get; set; }
}

public sealed class VerifyIpgProviderResponse : SamanProviderModelBaseResponse
{
    [JsonPropertyName("OrginalAmount")]
    public long OriginalAmount { get; set; }

    public string RRN { get; set; }
    public string RefNum { get; set; }
    public string MaskedPan { get; set; }
    public string HashedPan { get; set; }
    public int TerminalNumber { get; set; }
    public long AffectiveAmount { get; set; }
    public string StraceDate { get; set; }
    public string StraceNo { get; set; }
}

public sealed class ReverseIpgProviderRequest
{
    public string RefNum { get; set; }
    public string TerminalNumber { get; set; }
}

public sealed class ReverseIpgProviderResponse : SamanProviderModelBaseResponse
{
    [JsonPropertyName("OrginalAmount")]
    public long OriginalAmount { get; set; }

    public string RRN { get; set; }
    public string RefNum { get; set; }
    public string MaskedPan { get; set; }
    public string HashedPan { get; set; }
    public int TerminalNumber { get; set; }
    public long AffectiveAmount { get; set; }
    public string StraceDate { get; set; }
    public string StraceNo { get; set; }
}

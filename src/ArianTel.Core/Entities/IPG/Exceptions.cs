using ArianTel.Core.Abstractions;

namespace ArianTel.Core.Entities.Ipg;
public sealed class Exceptions : DbBaseEntity
{
    public string Code { get; set; }
    public string Message { get; set; }
    public int BankCode { get; set; }
    public string ProviderErrorCode { get; set; }
    public string ProviderErrorMessageEn { get; set; }
    public string ProviderErrorMessageFa { get; set; }
}

using System.ComponentModel.DataAnnotations;
using ArianTel.Core.Constants;
using ArianTel.Core.Enums.Ipg;

namespace ArianTel.API.Models.Ipg;

public sealed class InitIpgRequest
{
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public long Amount { get; set; }
    [MaxLength(512)]
    public string AdditionalData { get; set; }
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public int OrderId { get; set; }
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public string PhoneNumber { get; set; }
}

public sealed class CallBackFromSamanRequest
{
    public string MID { get; set; }
    public string State { get; set; }
    public int? Status { get; set; }
    public string RRN { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string TerminalId { get; set; }
    public string TraceNo { get; set; }
    public long Amount { get; set; }
    public long? Wage { get; set; }
    public string SecurePan { get; set; }
    public string HashedCardNumber { get; set; }
    public Bank Bank => Bank.Saman;
}

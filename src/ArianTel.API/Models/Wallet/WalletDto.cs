using System.ComponentModel.DataAnnotations;
using ArianTel.Core.Constants;

namespace ArianTel.API.Models.Wallet;

public sealed class BaseWalletDto
{
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [Range(1, 10000000000, ErrorMessage = AttributesErrorMessages.RangeMessage)]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    [MaxLength(200, ErrorMessage = AttributesErrorMessages.MaxLengthMessage)]
    [MinLength(30, ErrorMessage = AttributesErrorMessages.MinLengthMessage)]
    public string Description { get; set; }
}

public sealed class TransactionReportDto
{
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public int PageIndex { get; set; }
    [Required(ErrorMessage = AttributesErrorMessages.RequiredMessage)]
    public int PageSize { get; set; }
}

using System.ComponentModel.DataAnnotations;
using EnumFastToStringGenerated;

namespace ArianTel.Core.Enums.Ipg;
[EnumGenerator]
public enum IpgStatus
{
    None = 0,

    [Display(Name = "پرداخت باز")]
    Init = 1,

    [Display(Name = "بازگشته از درگاه")]
    ComeBackFromCallBack = 2,

    [Display(Name = "پرداخت موفق")]
    Succeeded = 3,

    [Display(Name = "پرداخت ناموفق")]
    Failed = 4,

    [Display(Name = "پرداخت برگشت داده شده")]
    Reversed = 5,

    [Display(Name = "پرداخت مسترد شده")]
    Refunded = 6
}

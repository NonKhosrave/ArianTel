using System.ComponentModel.DataAnnotations;

namespace ArianTel.Core.Enums.Wallet;
public enum BalanceOperation
{
    [Display(Name = "انتقال")]
    Transfer,
    [Display(Name = "شارژ ادمين")]
    Charge,
}

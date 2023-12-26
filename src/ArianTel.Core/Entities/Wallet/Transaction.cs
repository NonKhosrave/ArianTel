using System;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Enums.Wallet;

namespace ArianTel.Core.Entities.Wallet;
public class Transaction : DbBaseEntity
{
    public int SrcAccountId { get; set; }
    public int DesAccountId { get; set; }
    public decimal? SrcBalance { get; set; }
    public decimal DesBalance { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public BalanceOperation Operation { get; set; }
    public DateTime CreateDateTime { get; set; }
    public virtual Account SrcAccount { get; set; }
    public virtual Account DesAccount { get; set; }
}

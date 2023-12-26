using System;
using ArianTel.Core.Abstractions;

namespace ArianTel.Core.Entities.Wallet;
public class Transfer : DbBaseEntity
{
    public int AccountId { get; set; }
    public decimal Balance { get; set; }
    public decimal Amount { get; set; }
    public Guid TrackingNo { get; set; }
    public DateTime CreateDateTime { get; set; }
    public virtual Account Account { get; set; }
}

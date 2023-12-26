using System.Collections.Generic;
using ArianTel.Core.Abstractions;
using ArianTel.Core.Abstractions.AuditableEntity;

namespace ArianTel.Core.Entities.Wallet;
public class Account : DbBaseEntity, IAuditableEntity
{
    public int? UserId { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<Transfer> Transfers { get; set; }
    public virtual ICollection<Transaction> SrcTransactions { get; set; }
    public virtual ICollection<Transaction> DesTransactions { get; set; }
}

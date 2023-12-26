using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArianTel.Core.Entities.Wallet;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.DAL.EF.Context.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ArianTel.DAL.EF.Context.Repositories.Wallet;
public sealed class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Transaction>> GetTransactionReport(CancellationToken cancellationToken)
    {
        return await TableNoTracking.ToListAsync(cancellationToken);
    }
}

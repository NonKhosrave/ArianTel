using ArianTel.Core.Entities.Wallet;
using ArianTel.Core.Repositories.Wallet;
using ArianTel.DAL.EF.Context.DatabaseContext;

namespace ArianTel.DAL.EF.Context.Repositories.Wallet;
public sealed class TransferRepository : RepositoryBase<Transfer>, ITransferRepository
{
    public TransferRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
